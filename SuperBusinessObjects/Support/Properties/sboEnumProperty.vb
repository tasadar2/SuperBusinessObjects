Namespace Support

    Public Class sboEnumProperty
        Inherits sboProperty

        Public Property Options As sboOptions

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Enum
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Integer = -1,
                       Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = False, Optional DisplayProperty As sboProperty = Nothing,
                       Optional Options As sboOptions = Nothing)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)

            Me.Options = Options
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As String = Value
            Dim Result As New sboValidationResult

            Return Result
        End Function

        Public Function GetDisplay(Value As Integer) As String
            Dim Result As String = ""
            If Options.ContainsKey(Value) Then
                Result = Options(Value)
            End If
            Return Result
        End Function

        Public Shared Function GetDisplay(EnumProperty As sboEnumProperty, Value As Integer) As String
            Return EnumProperty.GetDisplay(Value)
        End Function

    End Class

End Namespace