Public Interface IsboDataContext

    'ReadOnly Property Publisher As sboPublisher

    Sub Load(Entity As sboEntity)
    Sub Load(EntityList As IsboList)
    Function Save(Entity As sboEntity) As Boolean
    Function Save(EntityList As IsboList) As Boolean

End Interface