using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    [SerializeField] int currentLevel = 1;
    [SerializeField] int totalExperience;
    [SerializeField] int previousLevelsExperience, nextLevelsExperience;
    [SerializeField] int expPerClick = 20;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    Slider experienceSlider;

    private void Awake()
    {
        experienceSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            AddExperience(expPerClick);
        }
    }

    private void Start()
    {
        experienceSlider.maxValue = experienceCurve.Evaluate(currentLevel);
        UpdateLevel();
    }
    
    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    void CheckForLevelUp()
    {
        while (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();

            //vfx sound
        }
    }

    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        experienceSlider.maxValue = end;
        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp /" + end + " exp";
        experienceSlider.value = start;
    }
}
