import { useState } from "react";
import { motion } from "framer-motion";
import {
  Mail,
  Phone,
  MapPin,
  Linkedin,
  Twitter,
  Github,
  Send,
  MessageCircle,
} from "lucide-react";

const contactInfo = [
  {
    icon: Mail,
    title: "Email",
    value: "soporte@jegasolutions.com",
    description: "Respuesta en 24 horas",
  },
  {
    icon: Phone,
    title: "Teléfono",
    value: "+57 (305) 123-4567",
    description: "Lun - Vie: 8:00 AM - 6:00 PM",
  },
  {
    icon: MapPin,
    title: "Ubicación",
    value: "Medellín, Colombia",
    description: "Oficina principal",
  },
];

const socialLinks = [
  { icon: Linkedin, href: "#", label: "LinkedIn" },
  { icon: Twitter, href: "#", label: "Twitter" },
  { icon: Github, href: "#", label: "GitHub" },
];

const Contact = () => {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    message: "",
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    await new Promise((resolve) => setTimeout(resolve, 1500));
    console.log("Form submitted:", formData);
    setIsSubmitting(false);
    setFormData({
      name: "",
      email: "",
      message: "",
    });
    alert("¡Gracias por tu mensaje! Te contactaremos pronto.");
  };

  return (
    <section className="section-padding bg-gradient-to-br from-blue-900 to-gray-900 text-white">
      <div className="container-max">
        <div className="flex flex-col h-full">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: -20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
            viewport={{ once: true }}
            className="text-center mb-6"
          >
            <h2 className="text-4xl md:text-5xl font-bold mb-3">
              Hablemos de tu <span className="gradient-text">Proyecto</span>
            </h2>
            <p className="text-lg text-gray-300 max-w-3xl mx-auto">
              Estamos listos para acompañarte en la transformación digital de tu
              empresa.
            </p>
          </motion.div>

          {/* Main Content */}
          <div className="flex-1 grid lg:grid-cols-5 gap-8">
            {/* Left Column - Contact Info */}
            <motion.div
              initial={{ opacity: 0, x: -30 }}
              whileInView={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.8 }}
              viewport={{ once: true }}
              className="lg:col-span-2 space-y-6"
            >
              <div>
                <h3 className="text-2xl font-bold mb-6">
                  Información de Contacto
                </h3>
                <div className="space-y-4">
                  {contactInfo.map((item, index) => (
                    <motion.div
                      key={item.title}
                      initial={{ opacity: 0, y: 20 }}
                      whileInView={{ opacity: 1, y: 0 }}
                      transition={{ duration: 0.6, delay: index * 0.1 }}
                      viewport={{ once: true }}
                      className="flex items-start space-x-3 group"
                    >
                      <div className="w-10 h-10 bg-jega-gold-400/20 rounded-lg flex items-center justify-center flex-shrink-0 group-hover:bg-jega-gold-400/30 transition-colors">
                        <item.icon className="w-5 h-5 text-jega-gold-400" />
                      </div>
                      <div>
                        <h4 className="font-semibold text-white mb-1 text-base">
                          {item.title}
                        </h4>
                        <p className="text-jega-gold-300 font-medium mb-1 text-base">
                          {item.value}
                        </p>
                        <p className="text-gray-400 text-xs">
                          {item.description}
                        </p>
                      </div>
                    </motion.div>
                  ))}
                </div>
              </div>

              {/* Social Links */}
              <div>
                <h4 className="text-xl font-semibold mb-4">Síguenos</h4>
                <div className="flex space-x-3">
                  {socialLinks.map((link, index) => (
                    <motion.a
                      key={link.label}
                      href={link.href}
                      initial={{ opacity: 0, scale: 0.8 }}
                      whileInView={{ opacity: 1, scale: 1 }}
                      transition={{ duration: 0.6, delay: index * 0.1 }}
                      viewport={{ once: true }}
                      className="w-10 h-10 bg-white/10 hover:bg-jega-gold-400/20 rounded-lg flex items-center justify-center transition-all duration-300 group hover:scale-110"
                    >
                      <link.icon className="w-4 h-4 text-white group-hover:text-jega-gold-400 transition-colors" />
                    </motion.a>
                  ))}
                </div>
              </div>
            </motion.div>

            {/* Right Column - Contact Form */}
            <motion.div
              initial={{ opacity: 0, x: 30 }}
              whileInView={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.8 }}
              viewport={{ once: true }}
              className="lg:col-span-3 bg-white/5 backdrop-blur rounded-xl p-6 border border-white/20"
            >
              <div className="flex items-center space-x-3 mb-4">
                <div className="w-8 h-8 bg-jega-gold-400 rounded-lg flex items-center justify-center">
                  <MessageCircle className="w-4 h-4 text-gray-900" />
                </div>
                <h3 className="text-2xl font-bold">
                  Solicita tu Demo Gratuita
                </h3>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid sm:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-300 mb-1">
                      Nombre completo *
                    </label>
                    <input
                      type="text"
                      name="name"
                      value={formData.name}
                      onChange={handleInputChange}
                      required
                      placeholder="Tu nombre completo"
                      className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-300 mb-1">
                      Correo electrónico *
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      required
                      placeholder="tu@email.com"
                      className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 text-sm"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">
                    Cuéntanos sobre tu proyecto *
                  </label>
                  <textarea
                    name="message"
                    value={formData.message}
                    onChange={handleInputChange}
                    required
                    rows="4"
                    placeholder="Describe tus necesidades específicas..."
                    className="w-full bg-white/10 border border-white/20 rounded-lg px-3 py-2 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-jega-gold-400 focus:border-transparent transition-all duration-300 resize-none text-sm"
                  ></textarea>
                </div>

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full btn-primary py-3 px-6 disabled:bg-gray-600 disabled:transform-none disabled:shadow-none flex items-center justify-center space-x-2 text-sm"
                >
                  {isSubmitting ? (
                    <>
                      <div className="w-4 h-4 border-2 border-gray-900 border-t-transparent rounded-full animate-spin"></div>
                      <span>Enviando...</span>
                    </>
                  ) : (
                    <>
                      <Send className="w-4 h-4" />
                      <span>Solicitar Demo Gratuita</span>
                    </>
                  )}
                </button>
              </form>
            </motion.div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Contact;
