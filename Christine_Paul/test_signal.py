from unittest import TestCase

from signal import Signal


class TestSignal(TestCase):
    def test_getter_returns_constructor(self):
        s = Signal(4)

        self.assertIs(s.get(), 4)
