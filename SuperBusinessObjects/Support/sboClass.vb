Imports Rock.QL.Mobius.SuperBusinessObjects.Support.sboFilter

Namespace Support

    Public Class sboClass
        Inherits Attribute
        Implements ICloneable

        Protected _ID As Guid = Guid.NewGuid
        Public ReadOnly Property ID As Guid
            Get
                Return _ID
            End Get
        End Property

        Public Property Owner As Type
        Public Property TableName As String
        Public Property [Alias] As String
        Public Property DisplayName As String
        Public Property PluralDisplayName As String
        Public Property PrimaryProperty As sboProperty
        Public Property PrimaryNameProperty As sboProperty
        Public Property DirectProperties As New List(Of sboProperty)
        Public Property NavigationProperties As New List(Of sboNavigationProperty)
        Public Property RelationshipsLoaded As Boolean

        Public ReadOnly Property Properties As IEnumerable(Of sboProperty)
            Get
                Return DirectProperties
            End Get
        End Property

        Public Sub New(Owner As Type, TableName As String, DisplayName As String, PluralDisplayName As String)
            GenerateNewHash()
            Me.Owner = Owner
            Me.TableName = TableName
            Me.Alias = TableName
            Me.DisplayName = DisplayName
            Me.PluralDisplayName = PluralDisplayName
        End Sub

        Public Function AddProperty(Of T As sboProperty)(Prop As T) As T
            Prop.Info = Me

            If PrimaryProperty Is Nothing Then
                Prop.IsPrimary = True
                PrimaryProperty = Prop
            End If
            If Prop.IsPrimaryName And PrimaryNameProperty Is Nothing Then
                PrimaryNameProperty = Prop
            End If
            DirectProperties.Add(Prop)
            Return Prop
        End Function

        Public Function CreateNew(DataContext As IsboDataContext) As sboEntity
            Dim Entity As sboEntity = Activator.CreateInstance(Owner, DataContext)
            Return Entity
        End Function

        Public Function CreateNewList(DataContext As IsboDataContext) As IsboList
            Dim List As IsboList = Activator.CreateInstance(GetType(sboList(Of )).MakeGenericType(Owner), DataContext)
            Return List
        End Function

        Public Function AddNavigationProperty(NavigationProperty As sboNavigationProperty) As sboNavigationProperty
            NavigationProperty.Info = Me
            NavigationProperties.Add(NavigationProperty)
            Return NavigationProperty
        End Function

        Private Shared HashKey As Integer = 1
        Private _HashCode As Integer
        Public Overrides Function GetHashCode() As Integer
            Return _HashCode
        End Function

        Private Sub GenerateNewHash()
            HashKey += 1
            _HashCode = HashKey
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is sboClass Then
                Return Me.ID = CType(obj, sboClass).ID
            End If
            Return False
        End Function

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim Copy As sboClass = Me.MemberwiseClone
            Copy.GenerateNewHash()
            Return Copy
        End Function

    End Class

End Namespace