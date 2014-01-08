Namespace Support

    Public Class sboGuidProperty
        Inherits sboProperty

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Guid
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Guid = Nothing,
                             Optional ShortDisplayName As String = "", Optional Primary As Boolean = False, Optional Required As Boolean = False, Optional Displayable As Boolean = False, Optional DisplayProperty As sboProperty = Nothing)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Primary:=Primary, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)

        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As Guid = Value
            Dim Result As New sboValidationResult
            If IsRequired Then
                If CoreValue = Guid.Empty Then
                    Result.Add(Me, String.Format("{0} is required", Me.DisplayName))
                End If
            End If
            Return Result
        End Function

        Public Overrides Property DefaultValue As Object
            Get
                If IsPrimary Then
                    Return Guid.NewGuid
                Else
                    Return MyBase.DefaultValue
                End If
            End Get
            Set(value As Object)
                MyBase.DefaultValue = value
            End Set
        End Property

    End Class

End Namespace