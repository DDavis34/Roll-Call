import { Label } from "./ui/label";
import { Button } from "./ui/button";
import { MinusCircle, PlusCircle } from "lucide-react";

interface AbilityScoresProps {
  scores: {
    strength: number;
    dexterity: number;
    constitution: number;
    intelligence: number;
    wisdom: number;
    charisma: number;
  };
  onScoreChange: (ability: string, value: number) => void;
}

const abilities = [
  { key: "strength", label: "Strength", abbr: "STR" },
  { key: "dexterity", label: "Dexterity", abbr: "DEX" },
  { key: "constitution", label: "Constitution", abbr: "CON" },
  { key: "intelligence", label: "Intelligence", abbr: "INT" },
  { key: "wisdom", label: "Wisdom", abbr: "WIS" },
  { key: "charisma", label: "Charisma", abbr: "CHA" }
];

function calculateModifier(score: number): string {
  const modifier = Math.floor((score - 10) / 2);
  return modifier >= 0 ? `+${modifier}` : `${modifier}`;
}

export function AbilityScores({ scores, onScoreChange }: AbilityScoresProps) {
  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
        {abilities.map(({ key, label, abbr }) => {
          const score = scores[key as keyof typeof scores];
          const modifier = calculateModifier(score);

          return (
            <div
              key={key}
              className="border rounded-lg p-4 space-y-2"
            >
              <div className="flex items-center justify-between">
                <div>
                  <Label className="text-xs text-gray-500">{abbr}</Label>
                  <div className="text-sm">{label}</div>
                </div>
                <div className="text-2xl font-mono">{modifier}</div>
              </div>

              <div className="flex items-center gap-2">
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => onScoreChange(key, Math.max(3, score - 1))}
                >
                  <MinusCircle className="w-4 h-4" />
                </Button>
                <div className="flex-1 text-center font-mono text-xl">{score}</div>
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => onScoreChange(key, Math.min(24, score + 1))}
                >
                  <PlusCircle className="w-4 h-4" />
                </Button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
