import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";

interface CharacterSheetProps {
  character: {
    name: string;
    level: number;
    ancestry: string;
    background: string;
    characterClass: string;
    abilityScores: {
      strength: number;
      dexterity: number;
      constitution: number;
      intelligence: number;
      wisdom: number;
      charisma: number;
    };
    skills: Array<{
      name: string;
      ability: string;
      proficiency: string;
    }>;
    feats: Array<{
      name: string;
      level: number;
      type: string;
    }>;
  };
}

function calculateModifier(score: number): string {
  const modifier = Math.floor((score - 10) / 2);
  return modifier >= 0 ? `+${modifier}` : `${modifier}`;
}

function getProficiencyBonus(proficiency: string, level: number): number {
  const bonuses = {
    untrained: 0,
    trained: level,
    expert: level + 2,
    master: level + 4,
    legendary: level + 6
  };
  return bonuses[proficiency as keyof typeof bonuses] || 0;
}

export function CharacterSheet({ character }: CharacterSheetProps) {
  const trainedSkills = character.skills.filter(
    (s) => s.proficiency !== "untrained"
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>Character Summary</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div>
          <div className="text-2xl mb-1">{character.name || "Unnamed Character"}</div>
          <div className="text-gray-600">
            Level {character.level} {character.ancestry} {character.characterClass}
          </div>
          {character.background && (
            <div className="text-sm text-gray-500">
              Background: {character.background}
            </div>
          )}
        </div>

        <div className="border-t pt-4">
          <div className="font-semibold mb-2">Ability Scores</div>
          <div className="grid grid-cols-3 gap-2">
            {Object.entries(character.abilityScores).map(([key, value]) => (
              <div key={key} className="text-sm">
                <span className="uppercase text-gray-500">
                  {key.substring(0, 3)}:
                </span>{" "}
                <span className="font-mono">{value}</span>{" "}
                <span className="text-gray-600">
                  ({calculateModifier(value)})
                </span>
              </div>
            ))}
          </div>
        </div>

        {trainedSkills.length > 0 && (
          <div className="border-t pt-4">
            <div className="font-semibold mb-2">Trained Skills</div>
            <div className="space-y-1">
              {trainedSkills.map((skill, index) => {
                const abilityMod = Math.floor(
                  (character.abilityScores[
                    skill.ability.toLowerCase() as keyof typeof character.abilityScores
                  ] -
                    10) /
                    2
                );
                const profBonus = getProficiencyBonus(
                  skill.proficiency,
                  character.level
                );
                const total = abilityMod + profBonus;

                return (
                  <div key={index} className="text-sm flex justify-between">
                    <span>
                      {skill.name}{" "}
                      <span className="text-gray-500 text-xs">
                        ({skill.proficiency})
                      </span>
                    </span>
                    <span className="font-mono">
                      {total >= 0 ? `+${total}` : total}
                    </span>
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {character.feats.length > 0 && (
          <div className="border-t pt-4">
            <div className="font-semibold mb-2">Feats</div>
            <div className="space-y-1">
              {character.feats.map((feat, index) => (
                <div key={index} className="text-sm">
                  {feat.name}{" "}
                  <span className="text-gray-500">
                    ({feat.type}, Lvl {feat.level})
                  </span>
                </div>
              ))}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
