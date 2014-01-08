Imports System.ComponentModel
Imports Rock.QL.Mobius.SuperBusinessObjects
Imports Rock.QL.Mobius.SuperBusinessObjects.Support

Public MustInherit Class sboEntity
    Implements INotifyPropertyChanging
    Implements INotifyPropertyChanged
    Implements ICloneable

    Public Enum sboEntityState
        NewEntity = 0
        Loaded = 1
        Changed = 2
        Deleted = 4
    End Enum

    Public Enum sboSaveState
        None = 0
        Create = 1
        Update = 2
        Delete = 4
    End Enum

    Public Event PropertyChanging(sender As Object, e As System.ComponentModel.PropertyChangingEventArgs) Implements System.ComponentModel.INotifyPropertyChanging.PropertyChanging
    Public Event PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Public Event Loaded(Entity As sboEntity)
    Public Event Saved(Entity As sboEntity)


#Region "Constructor"

    Public Sub New(DataContext As IsboDataContext)
        Me.new()
        Me.DataContext = DataContext
    End Sub

    Public Sub New(DataContext As IsboDataContext, ID As Object)
        Me.New(DataContext)
        Me.PrimaryID = ID
        Load()
    End Sub

    Public Sub New()
        If Info.RelationshipsLoaded = False Then
            Info.RelationshipsLoaded = True
            LoadRelationships()
        End If
        Query = New sboQuery(Info)
        Me.EntityState = sboEntityState.NewEntity
        Me.SaveState = sboSaveState.Create
    End Sub

    Protected Overridable Sub LoadRelationships()
    End Sub

#End Region



#Region "Properties"

    Public MustOverride ReadOnly Property Info As sboClass
    Public Property Query As sboQuery

    Protected ReadOnly _ClassName As String = Me.GetType.Name
    Public ReadOnly Property ClassName As String
        Get
            Return _ClassName
        End Get
    End Property

    Public Property PrimaryID As Object
        Get
            Return Me(Info.PrimaryProperty)
        End Get
        Set(value As Object)
            Me(Info.PrimaryProperty) = value
        End Set
    End Property

    Protected _LastValidation As sboValidationResult
    Public ReadOnly Property LastValidation As sboValidationResult
        Get
            Return _LastValidation
        End Get
    End Property

    Public ReadOnly Property IsValid As Boolean
        Get
            If LastValidation Is Nothing Then
                Me.Validate()
            End If

            Return Me.LastValidation.Success
        End Get
    End Property

    Public ReadOnly Property Properties As IEnumerable(Of sboProperty)
        Get
            Return Info.Properties
        End Get
    End Property

    Protected ReadOnly _Values As New Dictionary(Of sboProperty, Object)
    Protected ReadOnly Property Values As Dictionary(Of sboProperty, Object)
        Get
            Return _Values
        End Get
    End Property

    Default Public Property Item(Prop As sboProperty) As Object
        Get
            If Values.ContainsKey(Prop) = False Then
                Values(Prop) = Prop.DefaultValue
            End If

            Return Values(Prop)
        End Get
        Set(value As Object)
            OnPropertyChanging(Prop.PropertyName)
            Values(Prop) = value
            If LastValidation IsNot Nothing Then _LastValidation = Nothing
            If EntityState = sboEntityState.Loaded Then Me.EntityState = sboEntityState.Changed
            OnPropertyChanged(Prop.PropertyName)
        End Set
    End Property

    Protected ReadOnly _OriginalValues As New Dictionary(Of sboProperty, Object)
    Protected ReadOnly Property OriginalValues As Dictionary(Of sboProperty, Object)
        Get
            Return _OriginalValues
        End Get
    End Property

    Public Property OriginalItem(Prop As sboProperty) As Object
        Get
            Dim Result = Nothing
            If OriginalValues.ContainsKey(Prop) Then
                Result = OriginalValues(Prop)
            End If

            Return Result
        End Get
        Set(value As Object)
            OriginalValues(Prop) = value
        End Set
    End Property

    Public Property NavigationCollection As New Dictionary(Of sboNavigationProperty, NavigationEntity)
    Public Property NavigationParent(NavProp As sboNavigationProperty) As sboEntity
        Get
            Dim NavInfo As NavigationEntity = GetNavInfo(NavProp)
            If NavInfo.Loaded = False Then
                Select Case NavProp.LoadType
                    Case sboNavigationProperty.eLoadType.Auto
                        Dim Temp As sboEntity = NavProp.Relationship.ParentProperty.Info.CreateNew(Me.DataContext)
                        Temp(NavProp.Relationship.ParentProperty) = Me(NavProp.Relationship.ChildProperty)
                        Temp.Load()
                        If Temp.EntityState = sboEntityState.Loaded Then
                            NavInfo.Entity = Temp
                        End If
                        NavInfo.Loaded = True
                End Select
            End If

            Return NavInfo.Entity
        End Get
        Set(value As sboEntity)
            OnPropertyChanging(NavProp.PropertyName)
            Dim NavInfo As NavigationEntity = GetNavInfo(NavProp)
            NavInfo.Entity = value
            NavInfo.Loaded = True
            Me(NavProp.Relationship.ChildProperty) = NavInfo.Entity(NavProp.Relationship.ParentProperty)
            OnPropertyChanged(NavProp.PropertyName)
        End Set
    End Property

    Public ReadOnly Property NavigationPropertyLoaded(NavProp As sboNavigationProperty) As Boolean
        Get
            Dim NavInfo As NavigationEntity = GetNavInfo(NavProp)
            Return NavInfo.Loaded
        End Get
    End Property

    Public Property DataContext As IsboDataContext
    Public Property EntityState As sboEntityState
    Public Property SaveState As sboSaveState

#End Region



#Region "Support Methods"

    Public Sub OnPropertyChanging(PropertyName As String)
        RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(PropertyName))
    End Sub

    Public Sub OnPropertyChanged(PropertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(PropertyName))
    End Sub

    Public Sub OnLoaded()
        RaiseEvent Loaded(Me)
    End Sub

    Public Sub OnSaved()
        RaiseEvent Saved(Me)
    End Sub

#End Region



#Region "Methods"

    Public Overridable Sub Load()
        DataContext.Load(Me)
    End Sub

    Public Overridable Function Validate() As sboValidationResult
        Dim Result As New sboValidationResult
        For Each Prop In Properties
            Result.Add(Prop.Validate(Me(Prop)))
        Next
        _LastValidation = Result
        Return Result
    End Function

    Public Overridable Function Save() As Boolean
        Dim Result As Boolean = False

        If SaveState <> sboSaveState.None Then
            If Validate.Success Then
                DataContext.Save(Me)
                Result = True
            End If
        End If

        Return Result
    End Function

    Public Overridable Function Delete() As Boolean
        MarkDelete()
        Return Save()
    End Function

    Public Overridable Sub MarkDelete()
        Select Case EntityState
            Case sboEntityState.NewEntity, sboEntityState.Deleted
                Me.EntityState = sboEntityState.Deleted
                Me.SaveState = sboSaveState.None
            Case Else
                Me.SaveState = sboSaveState.Delete
        End Select
    End Sub

    Public Overridable Sub UnMarkDelete()
        Select Case EntityState
            Case sboEntityState.NewEntity, sboEntityState.Deleted
                EntityState = sboEntityState.NewEntity
                SaveState = sboSaveState.Create
            Case Else
                SaveState = sboSaveState.Update
        End Select
    End Sub

    Private Function GetNavInfo(NavProp As sboNavigationProperty) As NavigationEntity
        Dim NavInfo As NavigationEntity
        If NavigationCollection.ContainsKey(NavProp) Then
            NavInfo = NavigationCollection(NavProp)
        Else
            NavInfo = New NavigationEntity
            NavigationCollection.Add(NavProp, NavInfo)
        End If
        Return NavInfo
    End Function

    Public Sub LoadNavigationProperty(NavProp As sboNavigationProperty)
        Dim NavInfo As NavigationEntity = GetNavInfo(NavProp)
        Dim Temp As sboEntity = NavProp.Relationship.ParentProperty.Info.CreateNew(Nothing)
        Temp(NavProp.Relationship.ParentProperty) = Me(NavProp.Relationship.ChildProperty)
        Temp.Load()
        If Temp.EntityState = sboEntityState.Loaded Then
            NavInfo.Entity = Temp
        End If
        NavInfo.Loaded = True
    End Sub

#End Region



#Region "Replication Support"

    Private Function Clone1() As Object Implements System.ICloneable.Clone
        Return Clone()
    End Function

    ''' <summary>
    ''' Creates an exact clone of the entity.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Clone() As sboEntity
        Dim MyCopy As sboEntity = Activator.CreateInstance(Me.GetType, Me.DataContext)
        CloneTo(MyCopy)
        Return MyCopy
    End Function

    Public Sub CloneTo(Entity As sboEntity)
        Clone(Me, Entity)
    End Sub

    Public Sub CloneFrom(Entity As sboEntity)
        Clone(Entity, Me)
    End Sub

    Public Shared Sub Clone(FromEntity As sboEntity, ToEntity As sboEntity)
        For Each Prop In FromEntity.Properties
            ToEntity.Item(Prop) = FromEntity.Item(Prop)
            ToEntity.OriginalItem(Prop) = FromEntity.OriginalItem(Prop)
        Next
        ToEntity.EntityState = FromEntity.EntityState
        ToEntity.SaveState = FromEntity.SaveState
    End Sub

    ''' <summary>
    ''' Creates a duplicate of the entity.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Spawn() As sboEntity
        Dim MySpawn As sboEntity = Activator.CreateInstance(Me.GetType, Me.DataContext)
        SpawnTo(MySpawn)
        Return MySpawn
    End Function

    Public Sub SpawnTo(Entity As sboEntity)
        Spawn(Me, Entity)
    End Sub

    Public Sub SpawnFron(Entity As sboEntity)
        Spawn(Entity, Me)
    End Sub

    Public Shared Sub Spawn(FromEntity As sboEntity, ToEntity As sboEntity)
        ToEntity.EntityState = sboEntityState.NewEntity
        ToEntity.SaveState = sboSaveState.Create
        ToEntity.OriginalValues.Clear()
        ToEntity.Values.Clear()
        For Each Prop In FromEntity.Properties
            ToEntity.Values.Add(Prop, FromEntity.Item(Prop))
            ToEntity.OriginalValues.Add(Prop, FromEntity.OriginalItem(Prop))
        Next
        ToEntity.Values(ToEntity.Info.PrimaryProperty) = ToEntity.Info.PrimaryProperty.DefaultValue
    End Sub

#End Region

End Class
