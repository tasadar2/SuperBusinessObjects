Imports System.Text.RegularExpressions

Namespace Support

    Public Class sboFormattedValueProperty
        Inherits sboProperty

        Public Property FormProperty As sboProperty
        Public Property BaseProperty As sboProperty
        Public Property MaximumLength As Integer
        Public Property CharacterSet As String
        Public Property Format As String

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.String
            End Get
        End Property

        Public Sub New(FormProperty As sboStringProperty, BaseProperty As sboProperty, PropertyName As String, DisplayName As String, Optional DefaultValue As String = "",
                       Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing,
                       Optional MaximumLength As Integer = 0, Optional CharacterSet As String = "", Optional Format As String = "")

            MyBase.New("", PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, Computed:=True, DisplayProperty:=DisplayProperty)

            Me.FormProperty = FormProperty
            Me.BaseProperty = BaseProperty
            Me.MaximumLength = MaximumLength
            Me.CharacterSet = CharacterSet
            Me.Format = Format
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As String = Value
            Dim Result As New sboValidationResult
            Return Result
        End Function

        Public Function GetValue(Entity As sboEntity)
            Return Entity(FormProperty)
        End Function

        Public Sub SetValue(Entity As sboEntity, Value As String)
            Entity(FormProperty) = Regex.Replace(Value, "[^0-9.]", "")
        End Sub

    End Class

End Namespace