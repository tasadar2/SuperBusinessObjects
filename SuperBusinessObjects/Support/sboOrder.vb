Namespace Support

    Public Class sboOrder

        Public Enum sboDirection
            Ascending
            Descending
        End Enum

        Public Property Direction As sboDirection
        Public Property [Property] As sboProperty

        Public Sub New([Property] As sboProperty, Optional Direction As sboDirection = sboDirection.Ascending)
            Me.Property = [Property]
            Me.Direction = Direction
        End Sub

    End Class

End Namespace