from signal import Signal
from typing import Callable


class ComputedSignal[X, T](Signal):
    depends_on: Signal
    computed_by: Callable[[Signal], T]

    def __init__(self, depends_on: Signal[X], computed_by: Callable[[Signal[X]], T]):
        self.depends_on = depends_on
        self.computed_by = computed_by

        super().__init__(computed_by(depends_on))
