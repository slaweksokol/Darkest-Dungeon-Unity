﻿using UnityEngine;
using UnityEngine.UI;

public class CampingTrainerHeroWindow : MonoBehaviour
{
    [SerializeField]
    private Text heroNameLabel;
    [SerializeField]
    private Text heroClassLabel;
    [SerializeField]
    private Text classSkillLabel;
    [SerializeField]
    private Image heroGuildHeader;
    [SerializeField]
    private CampingSkillPurchaseSlot[] campingSkills;

    private TownManager TownManager { get; set; }
    private Hero ViewedHero { get; set; }

    public void Initialize(TownManager townManager)
    {
        TownManager = townManager;
        for (int i = 0; i < campingSkills.Length; i++)
            campingSkills[i].EventClicked += CampingSkillPurchaseSlotClicked;
    }

    public void LoadHeroOverview(Hero hero)
    {
        heroNameLabel.text = hero.HeroName;
        heroClassLabel.text = LocalizationManager.GetString("hero_class_name_" + hero.ClassStringId);
        classSkillLabel.text = LocalizationManager.GetString("action_verbose_body_camping_trainer_" + hero.ClassStringId);
        heroGuildHeader.sprite = DarkestDungeonManager.HeroSprites[hero.ClassStringId].Header;
        ViewedHero = hero;

        float discountCamping = 1 - DarkestDungeonManager.Campaign.Estate.CampingTrainer.Discount;
        int availableCampingSkills = Mathf.Min(campingSkills.Length, hero.HeroClass.CampingSkills.Count);
        for (int i = 0; i < availableCampingSkills; i++)
            campingSkills[i].Initialize(hero, i, discountCamping);
    }

    public void UpdateHeroOverview()
    {
        if (ViewedHero != null)
        {
            float discountCamping = 1 - DarkestDungeonManager.Campaign.Estate.CampingTrainer.Discount;
            int availableCampingSkills = Mathf.Min(campingSkills.Length, ViewedHero.HeroClass.CampingSkills.Count);
            for (int i = 0; i < availableCampingSkills; i++)
                campingSkills[i].UpdateSkill(discountCamping);
        }
    }

    public void ResetWindow()
    {
        ViewedHero = null;
        for (int i = 0; i < campingSkills.Length; i++)
            campingSkills[i].Reset();
    }

    private void CampingSkillPurchaseSlotClicked(CampingSkillPurchaseSlot slot)
    {
        float discount = 1 - DarkestDungeonManager.Campaign.Estate.CampingTrainer.Discount;

        if (DarkestDungeonManager.Campaign.Estate.BuyUpgrade(slot.Skill, slot.Hero, discount))
        {
            TownManager.EstateSceneManager.CurrencyPanel.UpdateCurrency();
            DarkestDungeonManager.Campaign.Estate.ReskillCampingHero(slot.Hero);
            UpdateHeroOverview();
            DarkestSoundManager.PlayOneShot("event:/town/trainer_purchase_skill");
        }
    }
}