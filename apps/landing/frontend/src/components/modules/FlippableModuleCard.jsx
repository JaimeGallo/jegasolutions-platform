import React, { useState } from "react";
import { motion } from "framer-motion";
import { RotateCcw, Info } from "lucide-react";

const FlippableModuleCard = ({ frontContent, backContent }) => {
  const [isFlipped, setIsFlipped] = useState(false);

  const handleCardClick = (e) => {
    // No voltear si el clic fue en un botón
    if (e.target.closest('button')) {
      return;
    }
    setIsFlipped(!isFlipped);
  };

  return (
    <div className="w-full min-h-[480px] [perspective:1000px] group">
      <motion.div
        className="relative w-full h-full cursor-pointer"
        style={{ transformStyle: "preserve-3d" }}
        animate={{ rotateY: isFlipped ? 180 : 0 }}
        transition={{ duration: 0.7, ease: "easeInOut" }}
        onClick={handleCardClick}
      >
        {/* Front of the card */}
        <div className="absolute w-full h-full bg-white rounded-2xl shadow-xl hover:shadow-2xl p-8 flex flex-col items-center justify-center text-center [backface-visibility:hidden] transition-shadow duration-300">
          {frontContent}
          
          {/* Indicador de clic para voltear */}
          <div className="absolute bottom-4 right-4 flex items-center space-x-2 text-gray-400 text-sm">
            <Info className="w-4 h-4" />
            <span>Haz clic para ver más</span>
          </div>
        </div>

        {/* Back of the card */}
        <div className="absolute w-full h-full bg-white rounded-2xl shadow-2xl p-8 [backface-visibility:hidden] [transform:rotateY(180deg)] overflow-y-auto">
          {backContent}
          
          {/* Botón para regresar */}
          <div className="absolute bottom-4 right-4">
            <div className="flex items-center space-x-2 text-gray-400 text-sm">
              <RotateCcw className="w-4 h-4" />
              <span>Haz clic para regresar</span>
            </div>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default FlippableModuleCard;
