from uuid import UUID

class Entity(object):
    """Represents an entity (as opposed to a value object)."""
    def __init__(self, id:UUID):
        assert isinstance(id, UUID)
        self.id = id
    def __str__(self):
        return '{}:id={}'.format(type(self).__name__, self.id)

class Aggregate(Entity):
    pass

class Value(object):
    pass

class Repository(object):
    """Repository pattern for Entity objects."""
    def __init__(self, entityType:type):
        assert issubclass(entityType, Entity)
        self.entityType = entityType
        from typing import Dict
        self.dict:Dict[UUID,Entity] = {}

    def __str__(self):
        return str(self.dict)

    def add(self, entity:Entity) -> None:
        assert isinstance(entity, Entity)
        assert isinstance(entity, self.entityType)
        self.dict[entity.id] = entity

    def get(self, id:UUID) -> Entity:
        assert isinstance(id, UUID)
        return self.dict[id]

if __name__ == '__main__':
    # test create an aggregate
    ag = Aggregate(UUID(int=0x12345678123456781234567812345678))
    print(ag)

    # test invalid ID should throw
    try:
        Aggregate('not a UUID')
    except AssertionError:
        print("Oops!  That was no valid UUID.  Try again...")

    # test repository with a few random entities
    test_rep = Repository(Entity)
    test_rep.add(Entity(UUID(int=0x12345678123456781234567812345678)))
    test_rep.add(Entity(UUID(int=0x12345678123456781234567812345679)))
    test_rep.add(Aggregate(UUID(int=0x12345678123456781234567812345680)))
    print(test_rep)
    print(test_rep.get(UUID(int=0x12345678123456781234567812345680)))
