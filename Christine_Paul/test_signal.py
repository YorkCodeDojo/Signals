from unittest import TestCase

from signal import Signal


class TestSignal(TestCase):
    def test_getter_returns_constructor(self):
        s = Signal(4)

        self.assertIs(s.get(), 4)

    def test_setter_then_returns_set_value(self):
        s = Signal(4)

        s.set(5)

        self.assertIs(s.get(), 5)
