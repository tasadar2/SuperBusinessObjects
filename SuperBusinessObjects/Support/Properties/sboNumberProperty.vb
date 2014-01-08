Namespace Support

    Public MustInherit Class sboNumberProperty(Of TNumber)
        Inherits sboProperty

        Public Property MinimumValue As TNumber
        Public Property MaximumValue As TNumber

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, DefaultValue As TNumber,
                       ShortDisplayName As String, Required As Boolean, Displayable As Boolean, DisplayProperty As sboProperty,
                       MinimumValue As TNumber, MaximumValue As TNumber)

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)

            Me.MinimumValue = MinimumValue
            Me.MaximumValue = MaximumValue

            If Comparer.Default.Compare(Me.MinimumValue, Me.MaximumValue) > 0 Then
                Throw New Exception("MinimumValue cannot be greater than MaximumValue")
            End If
        End Sub

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As TNumber = Value
            Dim Result As New sboValidationResult
            If IsRequired Then
                If CoreValue.Equals(0) Then
                    Result.Add(Me, String.Format("{0} is required", Me.DisplayName))
                End If
            End If
            If Comparer.Default.Compare(Value, MinimumValue) < 0 Then
                Result.Add(Me, String.Format("{0} cannot be less than {1}", Me.DisplayName, Me.MinimumValue))
            End If
            If Comparer.Default.Compare(Value, MaximumValue) > 0 Then
                Result.Add(Me, String.Format("{0} cannot be greater than {1}", Me.DisplayName, Me.MaximumValue))
            End If
            Return Result
        End Function

    End Class

End Namespace