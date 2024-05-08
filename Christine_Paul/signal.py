class Signal[T]:
    value: T

    def __init__(self, value: T):
        self.value = value

    def set(self, value: T):
        self.value = value

    def get(self) -> T:
        return self.value

    def __str__(self):
        return str(self.value)
