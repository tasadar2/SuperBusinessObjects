Namespace Support

    Public Class sboBooleanProperty
        Inherits sboProperty

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Boolean
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Boolean = False,
                       Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As Boolean = Value
            Dim Result As New sboValidationResult
            Return Result
        End Function

    End Class

End Namespace