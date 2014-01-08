Namespace Support

    Public Class sboDecimalProperty
        Inherits sboProperty

        Public Property MinimumValue As Decimal
        Public Property MaximumValue As Decimal
        Public Property Precision As Integer

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.Decimal
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As Integer = 0,
                             Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing,
                             Optional MinimumValue As Integer = Integer.MinValue, Optional MaximumValue As Integer = Integer.MaxValue)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)

            Me.MinimumValue = MinimumValue
            Me.MaximumValue = MaximumValue

            If Me.MinimumValue > Me.MaximumValue Then
                Throw New Exception("MinimumValue cannot be greater than MaximumValue")
            End If
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As Integer = Value
            Dim Result As New sboValidationResult
            If IsRequired Then
                If CoreValue = 0 Then
                    Result.Add(Me, String.Format("{0} is required", Me.DisplayName))
                End If
            End If
            If CoreValue < MinimumValue Then
                Result.Add(Me, String.Format("{0} cannot be less than {1}", Me.DisplayName, Me.MinimumValue))
            End If
            If CoreValue > MaximumValue Then
                Result.Add(Me, String.Format("{0} cannot be greater than {1}", Me.DisplayName, Me.MaximumValue))
            End If
            Return Result
        End Function

    End Class

End Namespace