Namespace Support

    Public Class sboValidationResult

        Public ReadOnly Property Success As Boolean
            Get
                Return Messages.Count = 0
            End Get
        End Property

        Public Property Messages As New List(Of sboFieldMessage)

        Public Sub Add(Prop As sboProperty, Message As String)
            Messages.Add(New sboFieldMessage(Prop, Message))
        End Sub

        Public Sub AddRange(Messages As IEnumerable(Of sboFieldMessage))
            Me.Messages.AddRange(Messages)
        End Sub

        Public Sub Add(ValidationResult As sboValidationResult)
            Me.Messages.AddRange(ValidationResult.Messages)
        End Sub

    End Class

    Public Class sboFieldMessage

        Public Property Message As String
        Public Property [Property] As sboProperty

        Public Sub New([Property] As sboProperty, Message As String)
            Me.Property = [Property]
            Me.Message = Message
        End Sub

    End Class

End Namespace