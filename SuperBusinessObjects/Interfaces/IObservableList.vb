Imports System.Collections.Specialized

Public Interface IObservableList
    Inherits INotifyCollectionChanged
    Inherits IList

End Interface

Public Interface IObservableList(Of Base)
    Inherits IObservableList
    Inherits IList(Of Base)

End Interface
