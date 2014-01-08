Namespace Support

    Public Class sboDateTimeProperty
        Inherits sboProperty

        Public Const DateTimeMinimum As DateTime = #1/1/1760#
        Public Property DisplayFormat As String

        Public Overrides ReadOnly Property Type As sboProperty.sboPropertyType
            Get
                Return sboPropertyType.DateTime
            End Get
        End Property

        Public Sub New(FieldName As String, PropertyName As String, DisplayName As String, Optional DefaultValue As DateTime = DateTimeMinimum,
                       Optional ShortDisplayName As String = "", Optional Required As Boolean = False, Optional Displayable As Boolean = True, Optional DisplayProperty As sboProperty = Nothing, Optional DisplayFormat As String = "")

            MyBase.New(FieldName, PropertyName, DisplayName, DefaultValue, ShortDisplayName:=ShortDisplayName, Required:=Required, Displayable:=Displayable, DisplayProperty:=DisplayProperty)
            Me.DisplayFormat = DisplayFormat

        End Sub

        Public Overrides Property DefaultValue As Object
            Get
                Dim Result = MyBase.DefaultValue
                If CType(MyBase.DefaultValue, DateTime) = DateTime.MaxValue Then
                    Result = Now
                End If
                Return Result
            End Get
            Set(value As Object)
                MyBase.DefaultValue = value
            End Set
        End Property

        Public Overrides Function Validate(Value As Object) As sboValidationResult
            Dim CoreValue As DateTime = Value
            Dim Result As New sboValidationResult
            Return Result
        End Function

    End Class

End Namespace