using System.Text.Json.Serialization;

namespace RollCallBackend.Models;

public class Creature
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("level")]
    public required string Level { get; set; }

    [JsonPropertyName("creature_family")]
    public required string CreatureFamily { get; set; }

    [JsonPropertyName("source")]
    public required string Source { get; set; }

    [JsonPropertyName("rarity")]
    public required string Rarity { get; set; }

    [JsonPropertyName("size")]
    public required string Size { get; set; }

    [JsonPropertyName("trait")]
    public required string Trait { get; set; }

    [JsonPropertyName("hp")]
    public required string Hp { get; set; }

    [JsonPropertyName("hp_scale")]
    public required string HpScale { get; set; }

    [JsonPropertyName("ac")]
    public required string Ac { get; set; }

    [JsonPropertyName("ac_scale")]
    public required string AcScale { get; set; }

    [JsonPropertyName("fortitude")]
    public required string Fortitude { get; set; }

    [JsonPropertyName("fortitude_scale")]
    public required string FortitudeScale { get; set; }

    [JsonPropertyName("reflex")]
    public required string Reflex { get; set; }

    [JsonPropertyName("reflex_scale")]
    public required string ReflexScale { get; set; }

    [JsonPropertyName("will")]
    public required string Will { get; set; }

    [JsonPropertyName("will_scale")]
    public required string WillScale { get; set; }

    [JsonPropertyName("immunity")]
    public required string Immunity { get; set; }

    [JsonPropertyName("resistance")]
    public required string Resistance { get; set; }

    [JsonPropertyName("weakness")]
    public required string Weakness { get; set; }

    [JsonPropertyName("creature_ability")]
    public required string CreatureAbility { get; set; }

    [JsonPropertyName("perception")]
    public required string Perception { get; set; }

    [JsonPropertyName("perception_scale")]
    public required string PerceptionScale { get; set; }

    [JsonPropertyName("sense")]
    public required string Sense { get; set; }

    [JsonPropertyName("speed")]
    public required string Speed { get; set; }

    [JsonPropertyName("attack_bonus")]
    public required string AttackBonus { get; set; }

    [JsonPropertyName("attack_bonus_scale")]
    public required string AttackBonusScale { get; set; }

    [JsonPropertyName("strike_damage_average")]
    public required string StrikeDamageAverage { get; set; }

    [JsonPropertyName("strike_damage_scale")]
    public required string StrikeDamageScale { get; set; }

    [JsonPropertyName("spell_attack")]
    public required string SpellAttack { get; set; }

    [JsonPropertyName("spell_attack_scale")]
    public required string SpellAttackScale { get; set; }

    [JsonPropertyName("spell_dc")]
    public required string SpellDc { get; set; }

    [JsonPropertyName("spell_dc_scale")]
    public required string SpellDcScale { get; set; }

    [JsonPropertyName("spell")]
    public required string Spell { get; set; }

    [JsonPropertyName("language")]
    public required string Language { get; set; }

    [JsonPropertyName("strength")]
    public required string Strength { get; set; }

    [JsonPropertyName("strength_scale")]
    public required string StrengthScale { get; set; }

    [JsonPropertyName("dexterity")]
    public required string Dexterity { get; set; }

    [JsonPropertyName("dexterity_scale")]
    public required string DexterityScale { get; set; }

    [JsonPropertyName("constitution")]
    public required string Constitution { get; set; }

    [JsonPropertyName("constitution_scale")]
    public required string ConstitutionScale { get; set; }

    [JsonPropertyName("intelligence")]
    public required string Intelligence { get; set; }

    [JsonPropertyName("intelligence_scale")]
    public required string IntelligenceScale { get; set; }

    [JsonPropertyName("wisdom")]
    public required string Wisdom { get; set; }

    [JsonPropertyName("wisdom_scale")]
    public required string WisdomScale { get; set; }

    [JsonPropertyName("charisma")]
    public required string Charisma { get; set; }

    [JsonPropertyName("charisma_scale")]
    public required string CharismaScale { get; set; }

    [JsonPropertyName("skill")]
    public required string Skill { get; set; }
}
