from unittest import TestCase

from computed_signal import ComputedSignal
from signal import Signal


class TestSignal(TestCase):
    def test_getter_returns_constructor(self):
        s = Signal(4)

        self.assertIs(s.get(), 4)

    def test_setter_then_returns_set_value(self):
        s = Signal(4)

        s.set(5)

        self.assertIs(s.get(), 5)

    def test_that_computed_signal_returns_constructor(self):
        s = ComputedSignal(Signal(4), lambda x: x.get())

        self.assertIs(s.get(), 4)

    def test_that_computed_signal_returns_dependent_value(self):
        # make a signal
        s = Signal(4)

        # make a computed signal that depends on the signal
        is_even = ComputedSignal(s, lambda signal: signal.get() % 2 == 0)

        self.assertIs(is_even.get(), True)

    def test_that_computed_signal_after_set_returns_dependent_value(self):
        # make a signal
        s = Signal(4)
        s.set(123124218421)

        # make a computed signal that depends on the signal
        is_even = ComputedSignal(s, lambda signal: signal.get() % 2 == 0)

        self.assertIs(is_even.get(), False)

    def test_that_computed_signal_returns_dependent_value_post_creation(self):
        # make a signal
        s = Signal(4)

        # make a computed signal that depends on the signal
        is_even = ComputedSignal(s, lambda signal: signal.get() % 2 == 0)
        s.set(123124218421)

        self.assertIs(is_even.get(), False)

    def test_complicated_computed_value(self):
        s = Signal(4)
        s1 = Signal(s)
        s2 = Signal(s1)

        is_even = ComputedSignal(s2, lambda signal: "True" if signal.get().get().get() % 2 == 0 else "You can't handle the truth")
        s.set(123124218421)

        self.assertIs(is_even.get(), "You can't handle the truth")
