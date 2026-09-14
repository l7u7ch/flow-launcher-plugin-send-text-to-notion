import unittest
import xml.etree.ElementTree as ET
from pathlib import Path


XAML_NAMESPACE = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
X_NAMESPACE = "http://schemas.microsoft.com/winfx/2006/xaml"
XAML_PATH = Path(__file__).parents[1] / "src" / "SettingsControl.xaml"


def elements(root: ET.Element, name: str) -> list[ET.Element]:
    return root.findall(f".//{{{XAML_NAMESPACE}}}{name}")


def named(element: ET.Element, name: str) -> bool:
    return element.get(f"{{{X_NAMESPACE}}}Name") == name


class SettingsControlLayoutTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = ET.parse(XAML_PATH).getroot()

    def test_api_token_uses_one_password_control(self) -> None:
        password_boxes = [
            element for element in elements(self.root, "PasswordBox")
            if named(element, "ApiTokenBox")
        ]
        visible_text_boxes = [
            element for element in elements(self.root, "TextBox")
            if named(element, "VisibleApiTokenBox")
        ]

        self.assertEqual(1, len(password_boxes))
        self.assertEqual([], visible_text_boxes)

    def test_api_token_visibility_uses_standard_checkbox(self) -> None:
        checkboxes = [
            element for element in elements(self.root, "CheckBox")
            if named(element, "ShowApiTokenCheckBox")
        ]
        custom_buttons = [
            element for element in elements(self.root, "Button")
            if named(element, "ToggleApiTokenVisibilityButton")
        ]

        self.assertEqual(1, len(checkboxes))
        self.assertEqual("Show token", checkboxes[0].get("Content"))
        self.assertEqual([], custom_buttons)

    def test_form_uses_flow_launcher_spacing_and_stretches(self) -> None:
        layout_grids = [
            element for element in elements(self.root, "Grid")
            if element.get("Margin") == "{StaticResource SettingPanelMargin}"
        ]

        self.assertEqual(1, len(layout_grids))
        self.assertIsNone(layout_grids[0].get("Width"))
        self.assertIsNone(layout_grids[0].get("MaxWidth"))
        self.assertNotEqual("Left", layout_grids[0].get("HorizontalAlignment"))


if __name__ == "__main__":
    unittest.main()
