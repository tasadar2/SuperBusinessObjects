Imports System.Runtime.Serialization
Imports Rock.QL.Mobius.SuperBusinessObjects.Support
Imports Rock.QL.Mobius.SuperBusinessObjects.Support.sboFilter
Imports System.Collections.Specialized
Imports System.ComponentModel

Public Class sboList(Of Base As {sboEntity, New})
    Inherits ObservableList(Of Base)
    Implements IsboList

#Region "Constructor"

    Public Sub New(DataContext As IsboDataContext)
        Me.DataContext = DataContext
        Dim Template As New Base
        Query = New sboQuery(Template.Info)
    End Sub

#End Region



#Region "Properties"

    Public Property DataContext As IsboDataContext Implements IsboList.DataContext

    Private ReadOnly _Type As Type = GetType(Base)
    Public ReadOnly Property Type As System.Type Implements IsboList.Type
        Get
            Return _Type
        End Get
    End Property

    Protected Friend _Info As sboClass
    Public ReadOnly Property Info As sboClass Implements IsboList.Info
        Get
            Return Query.Info
        End Get
    End Property

    Public Property Query As sboQuery Implements IsboList.Query

#End Region



#Region "Methods"

    Public Sub Where(Prop As sboProperty, Condition As sboFilterOperator, ParamArray Values() As Object)
        Query.Where(Prop, Condition, Values)
    End Sub

    Public Sub OrderBy(Prop As sboProperty, Optional Direction As sboOrder.sboDirection = sboOrder.sboDirection.Ascending)
        Query.OrderBy(Prop, Direction)
    End Sub

    Public Sub Load() Implements IsboList.Load
        DataContext.Load(Me)
    End Sub

    Public Function Save() Implements IsboList.Save
        For Each Entity In Me
            Entity.Save()
        Next
    End Function

    Public Function Validate() Implements IsboList.Validate
        For Each Entity In Me
            Entity.Validate()
        Next
    End Function

    Public Function Delete() Implements IsboList.Delete
        For Each Entity In Me
            Entity.Delete()
        Next
    End Function

    Public Function MarkDelete() Implements IsboList.MarkDelete
        For Each Entity In Me
            Entity.MarkDelete()
        Next
    End Function

#End Region

End Class
