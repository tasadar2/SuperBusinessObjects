Namespace Support

    Public MustInherit Class sboProperty
        Implements ICloneable

        Public Enum sboPropertyType
            [String]
            [Integer]
            [Long]
            [Decimal]
            [Enum]
            [Guid]
            [Boolean]
            [DateTime]
            Navigation
        End Enum

        Protected _ID As Guid = Guid.NewGuid
        Public ReadOnly Property ID As Guid
            Get
                Return _ID
            End Get
        End Property

        Public Property FieldName As String
        Public Property DisplayName As String
        Public Property PropertyName As String
        Public Property DisplayProperty As sboProperty = Me
        Public Property ShortDisplayName As String
        Public Property IsPrimary As Boolean
        Public Property IsPrimaryName As Boolean
        Public Property IsRequired As Boolean
        Public Property IsDisplayable As Boolean
        Public Property IsComputed As Boolean
        Public Property Info As sboClass
        Public Property ParentProperty As sboNavigationProperty
        Public Overridable Property DefaultValue As Object
        Public MustOverride ReadOnly Property Type As sboPropertyType

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, DefaultValue As Object,
                       Optional ShortDisplayName As String = "", Optional Primary As Boolean = False, Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional Computed As Boolean = False, Optional DisplayProperty As sboProperty = Nothing, Optional PrimaryName As Boolean = False)
            Me.New()

            Me.FieldName = FieldName
            Me.PropertyName = PropertyName
            Me.DisplayName = DisplayName
            Me.DefaultValue = DefaultValue

            Me.ShortDisplayName = ShortDisplayName
            If Me.ShortDisplayName.Length = 0 Then Me.ShortDisplayName = DisplayName
            Me.IsPrimary = Primary
            Me.IsPrimaryName = PrimaryName
            Me.IsRequired = Required
            Me.IsDisplayable = Displayable
            Me.IsComputed = Computed
            If DisplayProperty IsNot Nothing Then
                Me.DisplayProperty = DisplayProperty
            End If
        End Sub

        Public Sub New()
            GenerateNewHash()
        End Sub

        Public MustOverride Function Validate(Value As Object) As sboValidationResult

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
            If TypeOf obj Is sboProperty Then
                Return Me.ID = CType(obj, sboProperty).ID
            End If
            Return False
        End Function

        Public Overridable Function Clone() As Object Implements System.ICloneable.Clone
            Dim Copy As sboProperty = Me.MemberwiseClone
            Return Copy
        End Function

        Public Shared Function Create(Prop As sboProperty) As sboProperty
            Return Create({Prop})
        End Function

        Public Shared Function Create(NavigationProperty As sboNavigationProperty, Prop As sboProperty) As sboProperty
            Return Create({NavigationProperty, Prop})
        End Function

        Public Shared Function Create(NavigationProperty1 As sboNavigationProperty, NavigationProperty2 As sboNavigationProperty, Prop As sboProperty) As sboProperty
            Return Create({NavigationProperty1, NavigationProperty2, Prop})
        End Function

        Public Shared Function Create(ParamArray Props() As sboProperty) As sboProperty
            Dim Copy As sboProperty = Props.Last.Clone
            If Props.Length > 1 Then
                Copy.ParentProperty = Create(Props.Take(Props.Length - 1).ToArray)
                Copy.ParentProperty.ChildProperties.Add(Copy)
            End If
            Return Copy
        End Function

    End Class

End Namespace