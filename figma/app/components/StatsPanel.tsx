import { Circle } from "lucide-react";

interface StatsPanelProps {
  abilityScores: {
    strength: number;
    dexterity: number;
    constitution: number;
    intelligence: number;
    wisdom: number;
    charisma: number;
  };
}

export function StatsPanel({ abilityScores }: StatsPanelProps) {
  return (
    <div className="space-y-4">
      {/* Hero Points */}
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-red-600 flex items-center justify-center text-xs">
            ●
          </div>
          <span className="text-sm text-red-700">16</span>
        </div>
        <span className="text-sm text-[#8b7355]">Hero Points</span>
        <div className="flex gap-1 ml-auto">
          <Circle className="w-5 h-5 text-gray-400" />
          <Circle className="w-5 h-5 text-gray-400" />
          <Circle className="w-5 h-5 text-gray-400" />
        </div>
      </div>

      {/* Fighter DC */}
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-[#d8d0bf] flex items-center justify-center">
            <span className="text-xs text-[#5c4a3a]">⚔</span>
          </div>
          <span className="text-sm">+5</span>
        </div>
        <span className="text-sm text-[#8b7355]">Fighter DC</span>
      </div>

      {/* Perception */}
      <div className="flex items-center gap-3 border-t border-[#c4b8a0] pt-4">
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-[#d8d0bf] flex items-center justify-center">
            <span className="text-xs text-[#5c4a3a]">👁</span>
          </div>
          <span className="text-sm">+5</span>
        </div>
        <span className="text-sm text-[#8b7355]">Perception</span>
      </div>

      {/* Initiative */}
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <div className="w-6 h-6 rounded-full bg-[#d8d0bf] flex items-center justify-center">
            <span className="text-xs text-[#5c4a3a]">⚡</span>
          </div>
          <span className="text-sm">+0</span>
        </div>
        <span className="text-sm text-[#8b7355]">Initiative</span>
      </div>
    </div>
  );
}
