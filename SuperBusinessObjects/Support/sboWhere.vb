
Namespace Support

    Public Class sboWhere
        Implements ICloneable

        Public Enum sboWhereOperator
            [And]
            [Or]
        End Enum

        Public Property [Operator] As sboWhereOperator
        Public Property Wheres As New List(Of sboWhere)
        Public Property Filters As New List(Of sboFilter)

        Public ReadOnly Property HasFilters As Boolean
            Get
                Return Wheres.Count > 0 Or Filters.Count > 0
            End Get
        End Property

        Public Sub New(Optional [Operator] As sboWhereOperator = sboWhereOperator.And)
            Me.Operator = [Operator]
        End Sub

        Public Function Where(Prop As sboProperty, Condition As sboFilter.sboFilterOperator, ParamArray Values() As Object) As sboFilter
            Return Filters.Add(Prop, Condition, Values)
        End Function

        Public Function AddWhere(Optional [Operator] As sboWhereOperator = sboWhereOperator.And) As sboWhere
            Dim Where As New sboWhere([Operator])
            Wheres.Add(Where)
            Return Where
        End Function

        Public Function AddWhere(Where As sboWhere) As sboWhere
            Wheres.Add(Where)
            Return Where
        End Function

        Public Sub Clear()
            Wheres.Clear()
            Filters.Clear()
        End Sub

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim Result As sboWhere = Me.MemberwiseClone
            Result.Wheres = New List(Of sboWhere)
            Result.Filters = New List(Of sboFilter)
            For Each W In Wheres
                Result.Wheres.Add(W.Clone)
            Next
            For Each F In Filters
                Result.Filters.Add(F)
            Next
            Return Result
        End Function
    End Class

End Namespace