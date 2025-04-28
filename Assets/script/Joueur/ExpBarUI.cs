using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
	public Slider expSlider;
	public Text levelText;

	public void UpdateExp(float expPercentage)
	{
		expSlider.value = expPercentage;
	}

	public void UpdateLevel(int level)
	{
		levelText.text = "Lvl " + level;
	}
}