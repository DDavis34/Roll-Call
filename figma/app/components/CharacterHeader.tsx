import { Shield, Heart } from "lucide-react";
import { Input } from "./ui/input";

interface CharacterHeaderProps {
  name: string;
  level: number;
  abilityScores: {
    strength: number;
    dexterity: number;
    constitution: number;
    intelligence: number;
    wisdom: number;
    charisma: number;
  };
  onNameChange: (value: string) => void;
  onAbilityScoreChange: (scores: any) => void;
}

const calculateModifier = (score: number) => {
  return Math.floor((score - 10) / 2);
};

export function CharacterHeader({ name, level, abilityScores, onNameChange, onAbilityScoreChange }: CharacterHeaderProps) {
  const strMod = calculateModifier(abilityScores.strength);
  const dexMod = calculateModifier(abilityScores.dexterity);
  const conMod = calculateModifier(abilityScores.constitution);
  const intMod = calculateModifier(abilityScores.intelligence);
  const wisMod = calculateModifier(abilityScores.wisdom);
  const chaMod = calculateModifier(abilityScores.charisma);

  const ac = 14;
  const maxHp = 19;
  const currentHp = 19;

  return (
    <div className="flex flex-wrap gap-6">
      {/* Left Section - Basic Info */}
      <div className="flex gap-4">
        {/* Hide/Path buttons */}
        <div className="flex flex-col gap-2">
          <button className="text-xs text-[#8b7355] hover:text-[#5c4a3a] bg-[#f5f1e8] px-2 py-1 rounded border border-[#c4b8a0]">
            Hide
          </button>
          <button className="text-xs text-[#8b7355] hover:text-[#5c4a3a] bg-[#f5f1e8] px-2 py-1 rounded border border-[#c4b8a0]">
            Path
          </button>
        </div>

        {/* Level and Character Info */}
        <div>
          <div className="flex gap-4 mb-2">
            <div>
              <div className="text-xs text-[#8b7355]">Level</div>
              <div className="text-sm">{level}</div>
            </div>
            <div>
              <div className="text-xs text-[#8b7355]">XP</div>
              <div className="text-sm">0</div>
            </div>
          </div>

          {/* Character Name Input */}
          <div className="mt-2">
            <div className="text-xs text-[#8b7355] mb-1">Character Name</div>
            <Input
              value={name}
              onChange={(e) => onNameChange(e.target.value)}
              className="bg-[#f8f5ec] border-[#c4b8a0] text-sm h-8 w-48"
            />
          </div>

          {/* Size and Speed */}
          <div className="flex gap-4 mt-3">
            <div>
              <div className="text-xs text-red-700">SIZE</div>
              <div className="text-sm">Medium</div>
            </div>
            <div>
              <div className="text-xs text-red-700">SPEED</div>
              <div className="text-sm">25ft</div>
            </div>
          </div>
        </div>

        {/* AC Shield */}
        <div className="flex flex-col items-center">
          <div className="relative w-20 h-24 flex items-center justify-center">
            <Shield className="w-20 h-20 text-red-600" />
            <div className="absolute inset-0 flex flex-col items-center justify-center pt-1">
              <div className="text-xs text-red-700">AC</div>
              <div className="text-2xl">{ac}</div>
            </div>
          </div>
          <div className="mt-1 text-center">
            <div className="text-xs text-[#8b7355]">HP {currentHp}/{maxHp}</div>
            <div className="text-xs text-[#8b7355]">HP Shield</div>
          </div>
        </div>
      </div>

      {/* Ability Scores */}
      <div className="flex gap-3">
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">STR</div>
          <div className="text-lg">{abilityScores.strength}</div>
          <div className="text-sm text-red-700">{strMod >= 0 ? `+${strMod}` : strMod}</div>
        </div>
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">DEX</div>
          <div className="text-lg">{abilityScores.dexterity}</div>
          <div className="text-sm text-red-700">{dexMod >= 0 ? `+${dexMod}` : dexMod}</div>
        </div>
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">CON</div>
          <div className="text-lg">{abilityScores.constitution}</div>
          <div className="text-sm text-red-700">{conMod >= 0 ? `+${conMod}` : conMod}</div>
        </div>
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">INT</div>
          <div className="text-lg">{abilityScores.intelligence}</div>
          <div className="text-sm text-red-700">{intMod >= 0 ? `+${intMod}` : intMod}</div>
        </div>
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">WIS</div>
          <div className="text-lg">{abilityScores.wisdom}</div>
          <div className="text-sm text-red-700">{wisMod >= 0 ? `+${wisMod}` : wisMod}</div>
        </div>
        <div className="text-center">
          <div className="text-xs text-[#8b7355]">CHA</div>
          <div className="text-lg">{abilityScores.charisma}</div>
          <div className="text-sm text-red-700">{chaMod >= 0 ? `+${chaMod}` : chaMod}</div>
        </div>
      </div>

      {/* Saves */}
      <div className="flex gap-4 items-center">
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-red-600 flex items-center justify-center">
            <Heart className="w-4 h-4" />
          </div>
          <div>
            <div className="text-sm">+{6}</div>
            <div className="text-xs text-[#8b7355]">Fortitude</div>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-red-600 flex items-center justify-center">
            <Shield className="w-4 h-4" />
          </div>
          <div>
            <div className="text-sm">+{4}</div>
            <div className="text-xs text-[#8b7355]">Reflex</div>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-red-600 flex items-center justify-center">
            <span className="text-xs">✦</span>
          </div>
          <div>
            <div className="text-sm">+{3}</div>
            <div className="text-xs text-[#8b7355]">Will</div>
          </div>
        </div>
      </div>

      {/* Reset and Add Condition buttons */}
      <div className="ml-auto flex gap-2 items-start">
        <button className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] bg-[#f5f1e8] px-3 py-1 rounded border border-[#c4b8a0]">
          Rest
        </button>
        <button className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] bg-[#f5f1e8] px-3 py-1 rounded border border-[#c4b8a0]">
          Add Condition
        </button>
        <button className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] bg-[#f5f1e8] px-3 py-1 rounded border border-[#c4b8a0]">
          Add Custom Buff
        </button>
      </div>
    </div>
  );
}
