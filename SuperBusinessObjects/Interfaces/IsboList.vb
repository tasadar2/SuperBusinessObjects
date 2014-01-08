Imports Rock.QL.Mobius.SuperBusinessObjects.Support

Public Interface IsboList
    Inherits IObservableList

    Property DataContext As IsboDataContext
    ReadOnly Property Type As Type
    ReadOnly Property Info As sboClass

    Property Query As sboQuery

    Sub Load()

    Function Save()
    Function Validate()
    Function Delete()
    Function MarkDelete()

End Interface