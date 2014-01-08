'Imports System.ComponentModel
'Imports System.Windows.Threading

'Public Class sboPublisher

'    Public Property Dispatcher As Dispatcher

'    Private DeviceEventHandlerList As New List(Of EntityEventDelegate)

'    Public Delegate Sub EntityEventDelegate(Entity As sboEntity, EventType As sboEntity.sboSaveState)

'    Public Custom Event EntityEvent As EntityEventDelegate
'        AddHandler(value As EntityEventDelegate)
'            DeviceEventHandlerList.Add(value)
'        End AddHandler
'        RemoveHandler(value As EntityEventDelegate)
'            DeviceEventHandlerList.Remove(value)
'        End RemoveHandler
'        RaiseEvent(Entity As sboEntity, EventType As sboEntity.sboSaveState)
'            For Each Handler In DeviceEventHandlerList
'                If Handler IsNot Nothing And Dispatcher IsNot Nothing Then
'                    Dispatcher.BeginInvoke(Handler, New Object() {Entity, EventType})
'                End If
'            Next
'        End RaiseEvent
'    End Event

'    Public Sub New(Dispatcher As Dispatcher)
'        Me.Dispatcher = Dispatcher
'    End Sub

'    Public Sub OnEntityEvent(Entity As sboEntity, EventType As sboEntity.sboSaveState)
'        RaiseEvent EntityEvent(Entity, EventType)
'    End Sub

'End Class
