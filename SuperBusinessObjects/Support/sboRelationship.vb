Namespace Support

    Public Class sboRelationshipClass

        Public Property Relationships As New List(Of sboRelationshipDefinition)

        Public Function AddRelationship(Of T As sboRelationshipDefinition)(RelationshipDefinition As T) As T
            Relationships.Add(RelationshipDefinition)
            Return RelationshipDefinition
        End Function

    End Class

    Public Interface IsboRelationshipDefinition

        Property ParentProperty As sboProperty
        Property ChildProperty As sboProperty

    End Interface

    Public MustInherit Class sboRelationshipDefinition
        Implements IsboRelationshipDefinition

        Public Property ParentProperty As sboProperty Implements IsboRelationshipDefinition.ParentProperty
        Public Property ChildProperty As sboProperty Implements IsboRelationshipDefinition.ChildProperty

    End Class

    Public Class sboOneToManyRelationshipDefinition
        Inherits sboRelationshipDefinition

        Public Sub New(ParentProp As sboProperty, ChildProp As sboProperty)
            Me.ParentProperty = ParentProp
            Me.ChildProperty = ChildProp
        End Sub
    End Class

    Public Class sboManyToManyRelationshipDefinition
        Inherits sboRelationshipDefinition

        Public Property InnerInfo As sboClass
        Public Property ParentInnerProperty As sboProperty
        Public Property ChildInnerProperty As sboProperty

        Public Sub New(ParentProp As sboProperty, ParentInnerProp As sboProperty, ChildInnerProp As sboProperty, ChildProp As sboProperty)
            Me.ParentProperty = ParentProp
            Me.ParentInnerProperty = ParentInnerProp
            Me.ChildInnerProperty = ChildInnerProp
            Me.ChildProperty = ChildProp
            Me.InnerInfo = ParentInnerProperty.Info
        End Sub

    End Class

End Namespace