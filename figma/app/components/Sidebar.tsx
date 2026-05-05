import { Users, Briefcase, Sword, Settings } from "lucide-react";

interface SidebarProps {
  ancestry: string;
  background: string;
  characterClass: string;
  level: number;
  onAncestryChange: (value: string) => void;
  onBackgroundChange: (value: string) => void;
  onClassChange: (value: string) => void;
}

export function Sidebar({
  ancestry,
  background,
  characterClass,
  level,
  onAncestryChange,
  onBackgroundChange,
  onClassChange
}: SidebarProps) {
  return (
    <div className="p-3 space-y-3 text-xs">
      {/* Ancestry */}
      <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 cursor-pointer hover:border-[#a89768] transition-colors">
        <div className="flex items-center gap-2 mb-1">
          <Users className="w-3 h-3 text-red-700" />
          <div className="text-xs text-[#8b7355]">Ancestry</div>
        </div>
        <div className="text-xs text-[#5c4a3a]">{ancestry}</div>
      </div>

      {/* Background */}
      <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 cursor-pointer hover:border-[#a89768] transition-colors">
        <div className="flex items-center gap-2 mb-1">
          <Briefcase className="w-3 h-3 text-red-700" />
          <div className="text-xs text-[#8b7355]">Background</div>
        </div>
        <div className="text-xs text-[#5c4a3a]">{background}</div>
      </div>

      {/* Class */}
      <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 cursor-pointer hover:border-[#a89768] transition-colors">
        <div className="flex items-center gap-2 mb-1">
          <Sword className="w-3 h-3 text-red-700" />
          <div className="text-xs text-[#8b7355]">Class</div>
        </div>
        <div className="text-xs text-[#5c4a3a]">{characterClass}</div>
      </div>

      {/* Level 1 Section */}
      <div className="pt-3 border-t border-[#c4b8a0]">
        <div className="text-red-700 text-xs mb-2">Level 1</div>

        <div className="space-y-2">
          <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 flex items-center gap-2 cursor-pointer hover:border-[#a89768] transition-colors">
            <Settings className="w-4 h-4 text-[#8b7355]" />
            <div className="text-xs">Set Abilities</div>
          </div>

          <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 flex items-center gap-2 cursor-pointer hover:border-[#a89768] transition-colors">
            <Settings className="w-4 h-4 text-[#8b7355]" />
            <div className="text-xs">Skill Training</div>
          </div>

          <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-2 flex items-center gap-2 cursor-pointer hover:border-[#a89768] transition-colors">
            <Settings className="w-4 h-4 text-[#8b7355]" />
            <div className="text-xs">Class Skill</div>
          </div>
        </div>

        {/* Heritage */}
        <div className="mt-3 mb-2">
          <div className="flex items-center gap-2 mb-1">
            <Users className="w-3 h-3 text-red-700" />
            <div className="text-xs text-[#8b7355]">Heritage</div>
          </div>
          <div className="text-xs text-[#5c4a3a]">Not Selected</div>
        </div>

        {/* Ancestry Feat */}
        <div className="mt-3 mb-2">
          <div className="flex items-center gap-2 mb-1">
            <Users className="w-3 h-3 text-red-700" />
            <div className="text-xs text-[#8b7355]">Ancestry Feat</div>
          </div>
          <div className="text-xs text-[#5c4a3a]">Not Selected</div>
        </div>

        {/* Class Feat */}
        <div className="mt-3 mb-2">
          <div className="flex items-center gap-2 mb-1">
            <Sword className="w-3 h-3 text-red-700" />
            <div className="text-xs text-[#8b7355]">Class Feat</div>
          </div>
          <div className="text-xs text-[#5c4a3a]">Not Selected</div>
        </div>
      </div>

      {/* Reactive Strike Feature */}
      <div className="border-t border-[#c4b8a0] pt-3">
        <div className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] cursor-pointer py-1 px-2 hover:bg-[#f8f5ec] rounded transition-colors">
          Reactive Strike →
        </div>
      </div>

      {/* Free Feats */}
      <div className="space-y-1">
        <div className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] cursor-pointer py-1 px-2 hover:bg-[#f8f5ec] rounded transition-colors">
          Free Feat: Shield Block →
        </div>
        <div className="text-xs text-[#5c4a3a] hover:text-[#3d2f24] cursor-pointer py-1 px-2 hover:bg-[#f8f5ec] rounded transition-colors">
          Free Feat: Hobnobber
        </div>
      </div>

      {/* Level 2 */}
      <div className="pt-3 border-t border-[#c4b8a0]">
        <div className="text-red-700 text-xs mb-2">Level 2</div>
        <div className="mb-2">
          <div className="flex items-center gap-2 mb-1">
            <Sword className="w-3 h-3 text-red-700" />
            <div className="text-xs text-[#8b7355]">Class Feat</div>
          </div>
          <div className="text-xs text-[#5c4a3a]">Not Selected</div>
        </div>
        <div className="mb-2">
          <div className="flex items-center gap-2 mb-1">
            <Settings className="w-3 h-3 text-[#8b7355]" />
            <div className="text-xs text-[#8b7355]">Skill Feat</div>
          </div>
          <div className="text-xs text-[#5c4a3a]">Not Selected</div>
        </div>
      </div>

      {/* Level 3 */}
      <div className="pt-3 border-t border-[#c4b8a0]">
        <div className="text-red-700 text-xs mb-2">Level 3</div>
        <div className="bg-[#f8f5ec] border border-[#b8a882] rounded p-3 flex flex-col items-center justify-center cursor-pointer hover:border-[#a89768] transition-colors">
          <Settings className="w-6 h-6 text-[#8b7355]" />
          <div className="text-xs text-[#8b7355] mt-1">Skill Increase</div>
        </div>
      </div>
    </div>
  );
}
