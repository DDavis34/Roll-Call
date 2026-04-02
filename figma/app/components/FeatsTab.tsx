export function FeatsTab({ level }: { level: number }) {
  const feats = [
    { name: "Reactive Strike", type: "Class Feature", level: 1, description: "You can make an attack of opportunity when enemies within your reach use a manipulate action." },
    { name: "Shield Block", type: "Free Feat", level: 1, description: "You can use your shield to block incoming attacks." },
    { name: "Hobnobber", type: "Background Feat", level: 1, description: "You are skilled at gathering information in social situations." }
  ];

  return (
    <div className="space-y-3">
      {feats.map((feat, index) => (
        <div
          key={index}
          className="bg-[#f8f5ec] border border-[#c4b8a0] rounded p-4 hover:border-[#a89768] transition-colors"
        >
          <div className="flex items-start justify-between mb-2">
            <div>
              <div className="font-medium text-[#5c4a3a]">{feat.name}</div>
              <div className="text-xs text-[#8b7355]">{feat.type} • Level {feat.level}</div>
            </div>
          </div>
          <div className="text-sm text-[#5c4a3a] mt-2">{feat.description}</div>
        </div>
      ))}

      <div className="mt-6 pt-6 border-t border-[#c4b8a0]">
        <button className="w-full bg-[#f5f1e8] border border-[#c4b8a0] rounded px-4 py-3 text-left hover:border-[#a89768] transition-colors">
          <div className="text-sm text-[#5c4a3a]">+ Add Feat</div>
        </button>
      </div>
    </div>
  );
}
