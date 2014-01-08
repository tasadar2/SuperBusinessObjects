Namespace Support

    Public Class sboIntegerProperty
        Inherits sboNumberProperty(Of Integer)

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Integer
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Integer = 0,
                             Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing,
                             Optional MinimumValue As Integer = Integer.MinValue, Optional MaximumValue As Integer = Integer.MaxValue)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName, Required, Displayable, DisplayProperty, MinimumValue, MaximumValue)
        End Sub

    End Class

End Namespace