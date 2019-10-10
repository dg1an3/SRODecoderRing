from uuid import UUID
import numpy as np

from .common import Repository
from .TableDomainModel import TableModel

def sample_model_for_training(repo:Repository, id:UUID, count):
    """Service layer that will obtain a TableModel given an ID, and then produce count samples for training."""
    assert isinstance(repo, Repository)
    import copy
    tm:TableModel = copy.copy(repo.get(id))
    for z in range(count):
        tm.set_rxyz(0.0, 1.0, z)
        angles = (0.0, 1.0, z)
        
        # x vector is  matrix flattened + angles
        x = np.concatenate((tm.matrix.flatten(), np.array(angles))) 
        yield x, tm.rotation_order

if __name__ == '__main__':
    tm_repo = Repository(TableModel)
    uuids = [UUID(int=0x12345678123456781234567812345678),
             UUID(int=0x12345678123456781234567812345679),
             UUID(int=0x12345678123456781234567812345681)]
    tm_repo.add(TableModel(uuids[0], 'xyz'))
    tm_repo.add(TableModel(uuids[1], 'xzy'))
    tm_repo.add(TableModel(uuids[2], 'yxz'))
    print(tm_repo)

    for uuid in uuids:
        for dc in sample_model_for_training(tm_repo, uuid, 3):
            print(dc)