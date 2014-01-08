Namespace Support

    Public Class sboNavigationProperty
        Inherits sboProperty

        Public Enum eLoadType
            Auto
            Manual
            AsJoin
        End Enum

        Public Property [Alias] As String
            Get
                Return Relationship.ParentProperty.Info.Alias & Me.GetHashCode
            End Get
            Set(value As String)

            End Set
        End Property

        Public Property Relationship As sboRelationshipDefinition
        Public Property LoadType As eLoadType
        Public Property ChildProperties As New List(Of sboProperty)

        Public Sub New(PropertyName As String, Relationship As sboRelationshipDefinition)
            Me.PropertyName = PropertyName
            Me.Relationship = Relationship
        End Sub

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Navigation
            End Get
        End Property

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As Boolean = Value
            Dim Result As New sboValidationResult
            Return Result
        End Function

        Public Overrides Function Clone() As Object
            Dim Copy As sboNavigationProperty = MyBase.Clone()
            Copy.ChildProperties = New List(Of sboProperty)
            Return Copy
        End Function
    End Class

    Public Class NavigationEntity

        Public Property Relationship As sboRelationshipDefinition
        Public Property Loaded As Boolean
        Public Property Entity As sboEntity

    End Class

End Namespace