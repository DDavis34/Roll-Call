import { Circle } from "lucide-react";

export function WeaponsTab() {
  const weaponCategories = [
    { name: "Simple Weapons", active: true },
    { name: "Martial Weapons", active: true },
    { name: "Advanced Weapons", active: false },
    { name: "Unarmed Attacks", active: false }
  ];

  return (
    <div className="space-y-4">
      <div className="flex gap-2 flex-wrap">
        {weaponCategories.map((category, index) => (
          <button
            key={index}
            className={`px-3 py-1 rounded text-sm ${
              category.active
                ? "bg-red-600 text-white"
                : "bg-[#d8d0bf] text-[#8b7355]"
            }`}
          >
            <Circle className="w-3 h-3 inline mr-1" />
            {category.name}
          </button>
        ))}
      </div>

      <div className="mt-6">
        <button className="w-full bg-[#f8f5ec] border border-[#c4b8a0] rounded px-4 py-3 text-left hover:border-[#a89768] transition-colors">
          <div className="text-sm text-[#5c4a3a]">Add Weapon</div>
        </button>
      </div>

      <div className="text-center text-[#8b7355] text-sm mt-8">
        Print
      </div>

      <div className="text-[#8b7355] text-sm mt-4">
        No weapons equipped. Click "Add Weapon" to equip weapons.
      </div>
    </div>
  );
}
