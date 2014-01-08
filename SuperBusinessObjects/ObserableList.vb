Imports System.Collections.Specialized

Public Class ObservableList(Of Base)
    Implements IObservableList(Of Base)

    Private List As List(Of Base)
    Public Event CollectionChanged(sender As Object, e As System.Collections.Specialized.NotifyCollectionChangedEventArgs) Implements System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged

    Public Sub New()
        List = New List(Of Base)
    End Sub

    Protected Sub OnCollectionChanged(e As NotifyCollectionChangedEventArgs)
        RaiseEvent CollectionChanged(Me, e)
    End Sub


#Region "Collection Alteration"

    Public Overridable Sub Add(Item As Base) Implements System.Collections.Generic.ICollection(Of Base).Add
        List.Add(Item)
        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Item))
    End Sub

    Private Function Add1(Item As Object) As Integer Implements System.Collections.IList.Add
        Add(Item)
        Return IndexOf(Item)
    End Function

    Public Sub AddRange(Items As IEnumerable(Of Base))
        List.AddRange(Items)
    End Sub

    Public Sub Insert(Index As Integer, Item As Base) Implements System.Collections.Generic.IList(Of Base).Insert
        List.Insert(Index, Item)
        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Item, Index))
    End Sub

    Private Sub Insert1(Index As Integer, Item As Object) Implements System.Collections.IList.Insert
        Insert(Index, Item)
    End Sub

    Public Function Remove(Item As Base) As Boolean Implements System.Collections.Generic.ICollection(Of Base).Remove
        Dim Result = List.Remove(Item)

        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Item))
        Return Result
    End Function

    Private Sub Remove1(Item As Object) Implements System.Collections.IList.Remove
        Remove(Item)
    End Sub

    Public Sub RemoveAt(Index As Integer) Implements System.Collections.Generic.IList(Of Base).RemoveAt, System.Collections.IList.RemoveAt
        Dim Item = List(Index)
        List.RemoveAt(Index)
        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Item, Index))
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of Base).Clear, System.Collections.IList.Clear
        List.Clear()
        OnCollectionChanged(New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
    End Sub

#End Region



#Region "Collection Item Alteration"

    Public Function Contains(Item As Base) As Boolean Implements System.Collections.Generic.ICollection(Of Base).Contains
        Return List.Contains(Item)
    End Function

    Private Function Contains1(Item As Object) As Boolean Implements System.Collections.IList.Contains
        Return Contains(Item)
    End Function

    Public Function IndexOf(Item As Base) As Integer Implements System.Collections.Generic.IList(Of Base).IndexOf
        Return List.IndexOf(Item)
    End Function

    Private Function IndexOf1(Item As Object) As Integer Implements System.Collections.IList.IndexOf
        Return IndexOf(Item)
    End Function

    Default Public Property Item(Index As Integer) As Base Implements System.Collections.Generic.IList(Of Base).Item
        Get
            Return List(Index)
        End Get
        Set(value As Base)
            List(Index) = value
        End Set
    End Property

    Private Property Item1(Index As Integer) As Object Implements System.Collections.IList.Item
        Get
            Return Item(Index)
        End Get
        Set(value As Object)
            Item(Index) = value
        End Set
    End Property

#End Region



#Region "Collection Methods"

    Public Sub CopyTo(Array() As Base, ArrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of Base).CopyTo
        List.CopyTo(Array, ArrayIndex)
    End Sub

    Private Sub CopyTo1(Array As System.Array, ArrayIndex As Integer) Implements System.Collections.ICollection.CopyTo
        CType(List, ICollection).CopyTo(Array, ArrayIndex)
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of Base).Count, System.Collections.ICollection.Count
        Get
            Return List.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of Base).IsReadOnly, System.Collections.IList.IsReadOnly
        Get
            Return CType(List, ICollection(Of Base)).IsReadOnly
        End Get
    End Property

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of Base) Implements System.Collections.Generic.IEnumerable(Of Base).GetEnumerator
        Return List.GetEnumerator
    End Function

    Private Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    Private ReadOnly Property IsSynchronized As Boolean Implements System.Collections.ICollection.IsSynchronized
        Get
            Return CType(List, ICollection).IsSynchronized
        End Get
    End Property

    Private ReadOnly Property SyncRoot As Object Implements System.Collections.ICollection.SyncRoot
        Get
            Return CType(List, ICollection).SyncRoot
        End Get
    End Property

    Private ReadOnly Property IsFixedSize As Boolean Implements System.Collections.IList.IsFixedSize
        Get
            Return False
        End Get
    End Property

#End Region


End Class


