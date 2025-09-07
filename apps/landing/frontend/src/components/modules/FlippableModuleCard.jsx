import React, { useState } from "react";
import { motion } from "framer-motion";

const FlippableModuleCard = ({ frontContent, backContent }) => {
  const [isFlipped, setIsFlipped] = useState(false);

  return (
    <div
      className="w-full min-h-[480px] [perspective:1000px] group"
      onMouseEnter={() => setIsFlipped(true)}
      onMouseLeave={() => setIsFlipped(false)}
    >
      <motion.div
        className="relative w-full h-full"
        style={{ transformStyle: "preserve-3d" }}
        animate={{ rotateY: isFlipped ? 180 : 0 }}
        transition={{ duration: 0.7, ease: "easeInOut" }}
      >
        {/* Front of the card */}
        <div className="absolute w-full h-full bg-white rounded-2xl shadow-xl p-8 flex flex-col items-center justify-center text-center [backface-visibility:hidden]">
          {frontContent}
        </div>

        {/* Back of the card */}
        <div className="absolute w-full h-full bg-white rounded-2xl shadow-2xl p-8 [backface-visibility:hidden] [transform:rotateY(180deg)] overflow-y-auto">
          {backContent}
        </div>
      </motion.div>
    </div>
  );
};

export default FlippableModuleCard;
