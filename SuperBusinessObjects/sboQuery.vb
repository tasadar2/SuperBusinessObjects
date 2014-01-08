Imports Rock.QL.Mobius.SuperBusinessObjects.Support

Public Class sboQuery
    Inherits sboWhere

    Public Property Info As sboClass
    Public Property QueriedNavigationProperties As New List(Of sboNavigationProperty)
    Public Property ReturnedProperties As New List(Of sboProperty)
    Public Property Orders As New List(Of sboOrder)
    Public Property Limit As Integer
    Public Property Offset As Integer
    Public Property Distinct As Boolean

    Public Sub New(Info As sboClass)
        Me.Info = Info.Clone()
    End Sub

    Public Sub AddReturnedProperty(Prop As sboProperty)
        ReturnedProperties.Add(Prop)
        GetNavigationProperty(Prop)
    End Sub

    Public Sub AddReturnedPropertyRange(Props As IEnumerable(Of sboProperty))
        For Each Prop In Props
            AddReturnedProperty(Prop)
        Next
    End Sub

    Public Function GetNavigationProperty(Prop As sboProperty) As sboNavigationProperty
        Dim Result As sboNavigationProperty = Nothing
        Dim NavProp As sboNavigationProperty = Nothing
        If Prop.Type = sboProperty.sboPropertyType.Navigation Then
            NavProp = Prop
        ElseIf Prop.ParentProperty IsNot Nothing Then
            NavProp = Prop.ParentProperty
        End If
        If NavProp IsNot Nothing Then
            Result = GetNavigationProperty(NavProp)
        End If
        Return Result
    End Function

    Private Function GetNavigationProperty(Prop As sboNavigationProperty) As sboNavigationProperty
        Dim Result As sboNavigationProperty = Nothing

        If Prop.ParentProperty Is Nothing Then
            Result = QueriedNavigationProperties.Find(Prop.ID)
            If Result Is Nothing Then
                Result = Prop.Clone
                QueriedNavigationProperties.Add(Result)
            End If
        Else
            Dim Parent = GetNavigationProperty(Prop.ParentProperty)
            Result = Parent.ChildProperties.Find(Prop.ID)
            If Result Is Nothing Then
                Result = Prop.Clone
                Result.ParentProperty = Parent
                Parent.ChildProperties.Add(Result)
            End If
        End If

        Return Result
    End Function

    Public Sub OrderBy(Prop As sboProperty, Optional Direction As sboOrder.sboDirection = sboOrder.sboDirection.Ascending)
        Orders.Add(New sboOrder(Prop, Direction))
    End Sub

End Class
