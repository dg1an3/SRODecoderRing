from typing import List

def all_rotation_orders() -> List[str]:
    """Helper function to return all permutations of rotation orders."""
    from itertools import permutations
    return [''.join(axes) for axes in permutations({'x','y','z'})]

import numpy as np

def random_vector(n, mag) -> np.ndarray:
    """Helper function to generate a vector in random direction of given magnitude."""
    return 2.0 * mag * (0.5-np.random.random(size=n))

from uuid import UUID
from .common import Aggregate

class TableModel(Aggregate):
    """ """
    def __init__(self, id:UUID, rotation_order:str, noise:float = 0.1):
        super().__init__(id)
        assert rotation_order in all_rotation_orders()
        self.rotation_order = rotation_order
        self.noise = noise
        self.set_rxyz(0.0, 0.0, 0.0)

    def __str__(self):
        str_props= map(lambda tup: '{}={}'.format(tup[0], tup[1]),
                        [('rotation_order', self.rotation_order),
                            ('rotations', self.rotations),
                            ('matrix', self.matrix)])
        return "\n\t".join([super().__str__()] + list(str_props))
        # return '{}\n\trotation_order={}\n\trotations={}\n\tmatrix={}'.format(hdr, self.rotation_order, self.rotations, self.matrix)

    def set_rxyz(self, rx:float, ry:float, rz:float) -> None:
        self.rotations = {'x':rx,'y':ry,'z':rz}
        angles = np.array((rx,ry,rz)) + random_vector(3, self.noise)

        # generate matrix
        from scipy.spatial.transform import Rotation
        self.matrix = \
            Rotation.from_euler(self.rotation_order, 
                                angles, degrees = True).as_dcm()

if __name__ == '__main__':
    tm_repo = Repository(TableModel)
    uuids = [UUID(int=0x12345678123456781234567812345678),
             UUID(int=0x12345678123456781234567812345679),
             UUID(int=0x12345678123456781234567812345681)]
    tm_repo.add(TableModel(uuids[0], 'xyz'))
    tm_repo.add(TableModel(uuids[1], 'xzy'))
    tm_repo.add(TableModel(uuids[2], 'yxz'))
    print(tm_repo)
