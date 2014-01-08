Namespace Support

    Public Class sboStringProperty
        Inherits sboProperty

        Public Property MaximumLength As Integer
        Public Property CharacterSet As String

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.String
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As String = "",
                             Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing,
                             Optional MaximumLength As Integer = 0, Optional CharacterSet As String = "", Optional Format As String = "", Optional PrimaryName As Boolean = False)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty, PrimaryName:=PrimaryName)

            Me.MaximumLength = MaximumLength
            Me.CharacterSet = CharacterSet
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As String = Value
            Dim Result As New sboValidationResult
            If IsRequired Then
                If CoreValue.Length = 0 Then
                    Result.Add(Me, String.Format("{0} is required", Me.DisplayName))
                End If
            End If
            If MaximumLength > 0 Then
                If CoreValue.Length > MaximumLength Then
                    Result.Add(Me, String.Format("{0} cannot be longer than {1} characters", Me.DisplayName, Me.MaximumLength))
                End If
            End If
            If CharacterSet.Length > 0 Then
                'todo: detect improper characters
            End If
            Return Result
        End Function

    End Class

End Namespace