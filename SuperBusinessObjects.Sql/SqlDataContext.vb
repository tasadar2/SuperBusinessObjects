Imports Rock.Ql.Mobius.SuperBusinessObjects.Support
Imports System.Data.Common
Imports System.Data
Imports System.Text
Imports System.Threading

Public Class SqlDataContext
    Implements IsboDataContext

    Public Property CommandTimeout As Integer = 60
    Public Shared Property Errors As New Queue(Of Exception)

    Private _ConnectionString As String = ""
    Public ReadOnly Property ConnectionString As String
        Get
            Return _ConnectionString
        End Get
    End Property

    Private _Factory As DbProviderFactory
    Public ReadOnly Property Factory As DbProviderFactory
        Get
            Return _Factory
        End Get
    End Property

    'Private _Publisher As sboPublisher
    'Public ReadOnly Property Publisher As sboPublisher Implements IsboDataContext.Publisher
    '    Get
    '        Return _Publisher
    '    End Get
    'End Property

    Public Shared ReadOnly Property MSSql As DbProviderFactory
        Get
            Return DbProviderFactories.GetFactory("System.Data.SqlClient")
        End Get
    End Property

    Public Shared ReadOnly Property SQLite As DbProviderFactory
        Get
            Return DbProviderFactories.GetFactory("System.Data.SQLite")
        End Get
    End Property

    Public Sub New(Factory As DbProviderFactory, ConnectionString As String)
        _Factory = Factory
        _ConnectionString = ConnectionString
        '_Publisher = Publisher
    End Sub

    Private UniqueNumber As Integer = 0
    Private Function GetUniqueNumber() As Integer
        Dim Result = Interlocked.Increment(UniqueNumber)
        If Result = Integer.MaxValue Then
            Interlocked.Exchange(UniqueNumber, 0)
        End If
        Return Result
    End Function

    Public Sub Load(EntityList As IsboList) Implements IsboDataContext.Load
        Dim Filter = GenerateWhere(EntityList.Query)
        Dim Order = GenerateOrders(EntityList.Query)
        Dim Command = GenerateSelect(EntityList.Query)
        If Filter.CommandText <> "" Then
            Command.CommandText &= " where " & Filter.CommandText
            For Each Param As DbParameter In Filter.Parameters
                Command.Parameters.Add(CreateParameter(Param.ParameterName, Param.Value, Param.SourceColumn))
            Next
        End If
        Command.CommandText &= Order
        If EntityList.Query.Distinct Then
            Command.CommandText = Command.CommandText.Replace("select", "select distinct")
        End If
        Dim Data = OpenData(Command, EntityList.Query.Offset, EntityList.Query.Limit)
        EntityList.Clear()
        If Data IsNot Nothing Then
            For Each Row As DataRow In Data.Tables(0).Rows
                Dim Entity As sboEntity = EntityList.Info.CreateNew(Me)
                FillEntity(Entity.Info.Alias, Entity.Info, EntityList.Query.QueriedNavigationProperties, Row, Entity)
                EntityList.Add(Entity)
            Next
        End If
    End Sub

    Public Sub Load(Entity As sboEntity) Implements IsboDataContext.Load
        Entity.Query.Clear()
        Entity.Query.Where(Entity.Info.PrimaryProperty, sboFilter.sboFilterOperator.Equals, Entity(Entity.Info.PrimaryProperty))
        Dim Filter = GenerateWhere(Entity.Query)
        Dim Command = GenerateSelect(Entity.Query)
        If Filter.CommandText <> "" Then
            Command.CommandText &= " where " & Filter.CommandText
            For Each Param As DbParameter In Filter.Parameters
                Command.Parameters.Add(CreateParameter(Param.ParameterName, Param.Value, Param.SourceColumn))
            Next
        End If
        Dim Data = OpenData(Command, Entity.Query.Offset, Entity.Query.Limit)
        If Data IsNot Nothing Then
            FillEntity(Entity.Info.Alias, Entity.Info, Entity.Query.QueriedNavigationProperties, Data.Tables(0).Rows(0), Entity)
        End If
    End Sub

    Private Sub FillEntity([Alias] As String, Info As sboClass, ChildProperties As IEnumerable(Of sboProperty), Row As DataRow, Entity As sboEntity)
        For Each Prop In Info.DirectProperties.Where(Function(P) P.IsComputed = False)
            Dim Value As Object = Nothing
            Dim ColumnName As String = FilterName([Alias] & Prop.PropertyName)
            If Row.Table.Columns.Contains(ColumnName) Then
                If Row.IsNull(ColumnName) = False Then
                    Select Case Prop.Type
                        Case sboProperty.sboPropertyType.String
                            Value = Row(ColumnName)
                        Case sboProperty.sboPropertyType.Integer
                            Integer.TryParse(Row(ColumnName), Value)
                        Case sboProperty.sboPropertyType.Long
                            Long.TryParse(Row(ColumnName), Value)
                        Case sboProperty.sboPropertyType.Decimal
                            Decimal.TryParse(Row(ColumnName), Value)
                        Case sboProperty.sboPropertyType.Guid
                            If TypeOf Row(ColumnName) Is Guid Then
                                Value = Row(ColumnName)
                            End If
                        Case sboProperty.sboPropertyType.DateTime
                            If TypeOf Row(ColumnName) Is DateTime Then
                                Value = Row(ColumnName)
                            End If
                        Case sboProperty.sboPropertyType.Enum
                            Value = CInt(Row(ColumnName))
                        Case sboProperty.sboPropertyType.Boolean
                            Value = CBool(Row(ColumnName))
                        Case Else
                            Stop
                    End Select
                Else
                    Value = Prop.DefaultValue
                End If
                Entity(Prop) = Value
                Entity.OriginalItem(Prop) = Value
            End If
        Next

        For Each ChildProp As sboNavigationProperty In ChildProperties.Where(Function(C) C.Type = sboProperty.sboPropertyType.Navigation)
            Dim ChildEntity = ChildProp.Relationship.ParentProperty.Info.CreateNew(Entity.DataContext)
            FillEntity(ChildProp.Alias, ChildProp.Relationship.ParentProperty.Info, ChildProp.ChildProperties, Row, ChildEntity)
            Entity.NavigationParent(ChildProp) = ChildEntity
        Next

        Entity.EntityState = sboEntity.sboEntityState.Loaded
        Entity.SaveState = sboEntity.sboSaveState.Update
    End Sub

    Public Function Save(EntityList As IsboList) As Boolean Implements IsboDataContext.Save
        Dim Result As Boolean = True
        For Each Entity As sboEntity In EntityList
            If Save(Entity) = False Then
                Result = False
            End If
        Next
        Return Result
    End Function

    Public Function Save(Entity As sboEntity) As Boolean Implements IsboDataContext.Save
        Dim Result As Boolean = True
        Dim Command = Factory.CreateCommand

        Select Case Entity.SaveState
            Case sboEntity.sboSaveState.Create
                Dim Properties = Entity.Info.DirectProperties.Where(Function(P) P.IsReadOnly = False And P.IsComputed = False).ToList
                Dim Columns As New List(Of String)(Properties.Count)
                Dim Values As New List(Of String)(Properties.Count)

                For Each Prop In Properties
                    Dim Param = CreateParameter("@" & Prop.FieldName & GetUniqueNumber(), Entity.Item(Prop))
                    Columns.Add("[" & Prop.FieldName & "]")
                    Values.Add(Param.ParameterName)
                    Command.Parameters.Add(Param)
                Next

                Command.CommandText = "insert into [" & Entity.Info.TableName & "] (" & Join(Columns.ToArray, ", ") & ") output inserted.[" & Entity.Info.PrimaryProperty.FieldName & "] values (" & Join(Values.ToArray, ", ") & ")"

            Case sboEntity.sboSaveState.Update
                Dim Properties = Entity.Info.DirectProperties.Where(Function(P) P.IsReadOnly = False And P.IsComputed = False And P.IsPrimary = False).ToList
                Dim Sets As New List(Of String)(Properties.Count)

                For Each Prop In Properties
                    If Entity.Item(Prop).Equals(Entity.OriginalItem(Prop)) = False Then
                        Dim Param = CreateParameter("@" & Prop.FieldName & GetUniqueNumber(), Entity.Item(Prop))
                        Sets.Add("[" & Prop.FieldName & "] = " & Param.ParameterName)
                        Command.Parameters.Add(Param)
                    End If
                Next

                If Sets.Count > 0 Then
                    Dim PrimaryParam = CreateParameter("@" & Entity.Info.PrimaryProperty.FieldName & GetUniqueNumber(), Entity.PrimaryID)
                    Command.CommandText = "update [" & Entity.Info.TableName & "] set " & Join(Sets.ToArray, ", ") & " where [" & Entity.Info.PrimaryProperty.FieldName & "] = " & PrimaryParam.ParameterName
                    Command.Parameters.Add(PrimaryParam)
                End If

            Case sboEntity.sboSaveState.Delete
                Dim PrimaryParam = CreateParameter("@" & Entity.Info.PrimaryProperty.FieldName & GetUniqueNumber(), Entity.PrimaryID)
                Command.CommandText = "delete from [" & Entity.Info.TableName & "] where [" & Entity.Info.PrimaryProperty.FieldName & "] = " & PrimaryParam.ParameterName
                Command.Parameters.Add(PrimaryParam)
        End Select

        If Command.CommandText.Length > 0 Then
            Result = False
            Dim Data = OpenData(Command)

            If Data IsNot Nothing Then
                FillEntity("", Entity.Info, {}, Data.Tables(0).Rows(0), Entity)
                Result = True
            End If

            Dim EventType = Entity.SaveState

            If Result Then
                Entity.SaveState = sboEntity.sboSaveState.Update
                Entity.EntityState = sboEntity.sboEntityState.Loaded
                For Each Prop In Entity.Info.Properties
                    Entity.OriginalItem(Prop) = Entity.Item(Prop)
                Next
            End If

            'If Result And Publisher IsNot Nothing Then
            '    Publisher.OnEntityEvent(Entity, EventType)
            'End If
        End If


        Return Result
    End Function

    Private Function GenerateSelect(Query As sboQuery) As DbCommand
        Dim Command = Factory.CreateCommand
        Dim [Alias] As String = FormatAliasName(Query.Info.Alias)
        Dim Columns As IEnumerable(Of String) = New List(Of String)
        Dim Relationships As IEnumerable(Of String) = New List(Of String)

        GetRelations(Query.Info.Alias, Query.Info, Nothing, Query.QueriedNavigationProperties, "", Columns, Relationships)
        Command.CommandText = "select " & Join(Columns.ToArray, ", ") & " from " & FormatTableName(Query.Info.TableName) & " as " & [Alias] & " " & Join(Relationships.ToArray, " ")
        Return Command
    End Function

    Private Sub GetRelations([Alias] As String, Info As sboClass, Relationship As sboRelationshipDefinition, ChildProperties As IEnumerable(Of sboProperty), JoiningAlias As String, ByRef Columns As IEnumerable(Of String), ByRef Relationships As IEnumerable(Of String))
        Dim FormattedAlias As String = FormatAliasName([Alias])
        Columns = Columns.Concat(Info.DirectProperties.Where(Function(P) P.IsComputed = False).Select(Function(P) FormattedAlias & ".[" & P.FieldName & "] as " & FormatAliasName([Alias] & P.PropertyName)))
        If Relationship IsNot Nothing Then
            Relationships = Relationships.Concat({"left join " & FormatTableName(Info.TableName) & " as " & FormattedAlias & " on " & FormattedAlias & ".[" & Relationship.ParentProperty.FieldName & "] = " & FormatAliasName(JoiningAlias) & ".[" & Relationship.ChildProperty.FieldName & "]"})
        End If

        For Each ChildProp As sboNavigationProperty In ChildProperties.Where(Function(C) C.Type = sboProperty.sboPropertyType.Navigation)
            GetRelations(ChildProp.Alias, ChildProp.Relationship.ParentProperty.Info, ChildProp.Relationship, ChildProp.ChildProperties, [Alias], Columns, Relationships)
        Next
    End Sub

    Private Function FormatTableName(Table As String) As String
        Return "[" & Table.Replace(".", "].[") & "]"
    End Function

    Private Function FormatAliasName([Alias] As String) As String
        Return FormatTableName(FilterName([Alias]))
    End Function

    Private Function FilterName(Name As String) As String
        Return Name.Replace(".", "")
    End Function

    Private Function GenerateWhere(Query As sboQuery, Optional Where As sboWhere = Nothing) As DbCommand
        Dim Result = Factory.CreateCommand

        If Where Is Nothing Then
            Where = Query
        End If
        Dim Clauses As New List(Of String)
        For Each Filter As sboFilter In Where.Filters
            Dim Nav = Query.GetNavigationProperty(Filter.Prop)
            Dim [Alias] As String
            If Nav Is Nothing Then
                [Alias] = Query.Info.Alias
            Else
                [Alias] = Nav.Alias
            End If

            Dim W As String = FormatAliasName([Alias]) & ".[" & Filter.Prop.FieldName & "] "

            Dim Params As New List(Of DbParameter)
            For Each V In Filter.Values
                Params.Add(CreateParameter("@" & FilterName(Filter.Prop.Info.Alias) & Filter.Prop.FieldName & GetUniqueNumber(), V))
            Next

            Select Case Filter.Operator
                Case sboFilter.sboFilterOperator.Equals, sboFilter.sboFilterOperator.In
                    If Params.Count <> 1 Then
                        W &= "in"
                    Else
                        W &= "="
                    End If
                Case sboFilter.sboFilterOperator.NotIn, sboFilter.sboFilterOperator.NotEquals
                    If Params.Count <> 1 Then
                        W &= "not in"
                    Else
                        W &= "<>"
                    End If
                Case sboFilter.sboFilterOperator.Like
                    W &= "like"
                Case sboFilter.sboFilterOperator.NotLike
                    W &= "not like"
                Case sboFilter.sboFilterOperator.Null
                    W &= "is null"
                Case sboFilter.sboFilterOperator.NotNull
                    W &= "is not null"
            End Select

            If Params.Count > 0 Then
                Result.Parameters.AddRange(Params.ToArray)
                Dim Test = " " & Join(Params.Select(Function(P) P.ParameterName).ToArray, ", ")
                If Params.Count <> 1 Then
                    Test = " (" & Test & ")"
                End If
                W &= Test
            End If

            Clauses.Add(W)
        Next

        For Each W In Where.Wheres
            Dim Command = GenerateWhere(Query, W)

            If Command.CommandText.Length > 0 Then
                Clauses.Add(Command.CommandText)
            End If

            For Each Param As DbParameter In Command.Parameters
                Result.Parameters.Add(CreateParameter(Param.ParameterName, Param.Value, Param.SourceColumn))
            Next
        Next

        If Clauses.Count > 0 Then
            Result.CommandText = "(" & Join(Clauses.ToArray, ") " & Where.Operator.ToString & " (") & ")"
        End If

        Return Result
    End Function

    Private Function GenerateOrders(Query As sboQuery) As String
        Dim Result As New StringBuilder
        Dim Append As Boolean
        If Query.Orders.Count > 0 Then
            Result.Append(" Order By ")
            For Each Order In Query.Orders
                Dim Nav = Query.GetNavigationProperty(Order.Property)
                Dim [Alias] As String = ""
                If Nav IsNot Nothing Then
                    [Alias] = Nav.Alias
                Else
                    [Alias] = Query.Info.Alias
                End If
                If Append Then
                    Result.Append(", ")
                Else
                    Append = True
                End If
                Result.Append(FormatAliasName([Alias]) & ".[" & Order.Property.FieldName & "] " & GenerateDirection(Order.Direction))
            Next
        End If
        Return Result.ToString
    End Function

    Private Function GenerateDirection(Direction As sboOrder.sboDirection) As String
        Dim Result As String = "Asc"
        If Direction = sboOrder.sboDirection.Descending Then
            Result = "Desc"
        End If
        Return Result
    End Function

    Private Function OpenData(ByVal Command As DbCommand, Optional Offset As Integer = 0, Optional Limit As Integer = 0) As DataSet
        Return OpenDataRaw(Factory, ConnectionString, Command, CommandTimeout, Offset, Limit)
    End Function

    Private Shared Function OpenDataRaw(ByVal Factory As DbProviderFactory, ByVal ConnectionString As String, ByVal Command As DbCommand, ByVal CommandTimeout As Integer, Offset As Integer, Limit As Integer) As DataSet
        Dim Result As New DataSet
        Using Conn As DbConnection = Factory.CreateConnection
            Conn.ConnectionString = ConnectionString
            Using DA As DbDataAdapter = Factory.CreateDataAdapter
                DA.SelectCommand = Command
                DA.SelectCommand.Connection = Conn
                If DA.SelectCommand.CommandTimeout < CommandTimeout Then DA.SelectCommand.CommandTimeout = CommandTimeout
                Try
                    Conn.Open()
                    If DA.Fill(Result, Offset, Limit, "Table") = 0 Then Result = Nothing
                Catch ex As System.Exception
                    Result = Nothing
                    Errors.Enqueue(ex)
                Finally
                    Conn.Close()
                End Try
            End Using
        End Using
        Return Result
    End Function

    Private Function ExecuteData(ByVal Command As DbCommand) As Integer
        Return ExecuteDataRaw(Factory, ConnectionString, Command, CommandTimeout)
    End Function

    Private Shared Function ExecuteDataRaw(ByVal Factory As DbProviderFactory, ByVal ConnectionString As String, ByVal Command As DbCommand, ByVal CommandTimeout As Integer) As Integer
        Dim Result As Integer = 0
        Using Conn As DbConnection = Factory.CreateConnection
            Conn.ConnectionString = ConnectionString
            Command.Connection = Conn
            If Command.CommandTimeout < CommandTimeout Then Command.CommandTimeout = CommandTimeout
            Try
                Conn.Open()
                Result = Command.ExecuteNonQuery
            Catch ex As System.Exception
                Errors.Enqueue(ex)
            Finally
                Conn.Close()
            End Try
        End Using
        Return Result
    End Function

    Public Function CreateParameter(ByVal Name As String, ByVal Value As Object, Optional ByVal SourceColumn As String = "") As DbParameter
        Dim Param As DbParameter = Factory.CreateParameter
        Param.Direction = ParameterDirection.Input
        Param.ParameterName = Name
        Param.Value = Value
        If SourceColumn IsNot Nothing AndAlso SourceColumn.Length > 0 Then Param.SourceColumn = SourceColumn
        Return Param
    End Function

End Class