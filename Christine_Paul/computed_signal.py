from signal import Signal
from typing import Callable


class ComputedSignal[X, T](Signal):
    depends_on: Signal[X]
    last_value: X
    flat_last: str
    computed_by: Callable[[Signal], T]

    def __init__(self, depends_on: Signal[X], computed_by: Callable[[Signal[X]], T]):
        self.depends_on = depends_on
        self.computed_by = computed_by
        self.last_value = depends_on.get()
        self.flat_last = str(self.last_value)

        super().__init__(computed_by(depends_on))

    def get(self) -> T:
        if self.flat_last != str(self.depends_on.get()):
            self.last_value = self.depends_on.get()
            self.set(self.computed_by(self.depends_on))

        return super().get()
