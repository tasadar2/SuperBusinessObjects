'Public Class sboTransaction
'    Implements IsboDataContext

'    Public Property DataContext As IsboDataContext

'    Private Saves As New Dictionary(Of String, Dictionary(Of Guid, sboEntity))

'    Public Sub New(DataContext As IsboDataContext)
'        Me.DataContext = DataContext
'    End Sub

'    Public Sub Load(EntityList As IsboList) Implements IsboDataContext.Load
'        DataContext.Load(EntityList)
'        For Each Entity As sboEntity In EntityList
'            Entity.DataContext = Me
'        Next
'        If Saves.ContainsKey(EntityList.Info.TableName) Then
'            Dim SaveTable = Saves(EntityList.Info.TableName)
'            For Each Entity As sboEntity In SaveTable.Values

'            Next
'        End If
'    End Sub

'    Public Sub Load(Entity As sboEntity) Implements IsboDataContext.Load
'        If Saves.ContainsKey(Entity.Info.TableName) AndAlso Saves(Entity.Info.TableName).ContainsKey(Entity.PrimaryID) Then
'            Entity.CloneFrom(Saves(Entity.Info.TableName)(Entity.PrimaryID))
'        Else
'            DataContext.Load(Entity)
'            Entity.DataContext = Me
'        End If
'    End Sub

'    Public ReadOnly Property Publisher As sboPublisher Implements IsboDataContext.Publisher
'        Get
'            Return DataContext.Publisher
'        End Get
'    End Property

'    Public Function Save(EntityList As IsboList) As Boolean Implements IsboDataContext.Save
'        For Each Entity As sboEntity In EntityList
'            Save(Entity)
'        Next
'    End Function

'    Public Function Save(Entity As sboEntity) As Boolean Implements IsboDataContext.Save
'        Dim TableName As String = Entity.Info.TableName
'        If Saves.ContainsKey(TableName) = False Then
'            Saves.Add(TableName, New Dictionary(Of Guid, sboEntity))
'        End If
'        If Saves(TableName).ContainsKey(Entity.PrimaryID) Then
'            Saves(TableName)(Entity.PrimaryID) = Entity
'        Else
'            Saves(TableName).Add(Entity.PrimaryID, Entity)
'        End If
'    End Function

'    Public Function SaveTransaction()
'        For Each Table In Saves.Values
'            For Each TableSave In Table.Values
'                DataContext.Save(TableSave)
'            Next
'        Next
'    End Function

'End Class
