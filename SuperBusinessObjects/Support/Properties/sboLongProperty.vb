Namespace Support

    Public Class sboLongProperty
        Inherits sboNumberProperty(Of Long)

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Long
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Long = 0,
                             Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing,
                             Optional MinimumValue As Long = Long.MinValue, Optional MaximumValue As Long = Long.MaxValue)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName, Required, Displayable, DisplayProperty, MinimumValue, MaximumValue)
        End Sub

    End Class

End Namespace