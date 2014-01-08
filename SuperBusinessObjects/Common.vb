Imports System.Runtime.CompilerServices
Imports Rock.QL.Mobius.SuperBusinessObjects.Support


Public Module SuperBusinessObjects

    <Extension()>
    Public Function Find(List As IEnumerable(Of sboProperty), ID As Guid) As sboProperty
        Return List.Where(Function(P) P.ID = ID).FirstOrDefault
    End Function


End Module

