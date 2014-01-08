Imports System.Runtime.CompilerServices

Namespace Support

    Public Class sboFilter
        Public Enum sboFilterOperator
            Equals
            NotEquals
            [In]
            NotIn
            [Like]
            NotLike
            Null
            NotNull
        End Enum

        Public Property Prop As sboProperty
        Public Property [Operator] As sboFilterOperator
        Public Property Values As New List(Of Object)

        Public Sub New(Prop As sboProperty, [Operator] As sboFilterOperator, ParamArray Values() As Object)
            Me.Prop = Prop
            Me.Operator = [Operator]
            Me.Values.AddRange(Values)
        End Sub

    End Class

    Public Module FilterExtensions

        <Extension()>
        Public Function Add(List As List(Of sboFilter), Prop As sboProperty, Condition As sboFilter.sboFilterOperator, ParamArray Values() As Object) As sboFilter
            Dim Filter As New sboFilter(Prop, Condition, Values)
            List.Add(Filter)
            Return Filter
        End Function

    End Module

End Namespace