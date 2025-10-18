#!/bin/bash

echo "🔍 Validando configuración JWT en todos los módulos..."
echo ""

# Función para verificar configuración en appsettings.json
check_jwt_config() {
    local file=$1
    local module=$2

    if [ -f "$file" ]; then
        echo "📁 Verificando: $module"

        # Verificar SecretKey
        secret=$(grep '"SecretKey"' "$file" | sed 's/.*"SecretKey": *"\([^"]*\)".*/\1/')
        echo "   SecretKey: $secret"

        # Verificar Issuer
        issuer=$(grep '"Issuer"' "$file" | sed 's/.*"Issuer": *"\([^"]*\)".*/\1/')
        echo "   Issuer: $issuer"

        # Verificar Audience
        audience=$(grep '"Audience"' "$file" | sed 's/.*"Audience": *"\([^"]*\)".*/\1/')
        echo "   Audience: $audience"
        echo ""

        # Validar valores esperados
        expected_secret="YOUR_JWT_SECRET_HERE"
        expected_issuer="JEGASolutions.Landing.API"
        expected_audience="jegasolutions-landing-client"

        if [[ "$secret" == *"$expected_secret"* ]] && \
           [[ "$issuer" == *"$expected_issuer"* ]] && \
           [[ "$audience" == *"$expected_audience"* ]]; then
            echo "   ✅ Configuración correcta"
        else
            echo "   ❌ Configuración incorrecta - revisar valores"
        fi
        echo "---"
    else
        echo "⚠️  Archivo no encontrado: $file"
        echo "---"
    fi
}

# Verificar Landing API
check_jwt_config "apps/landing/backend/src/JEGASolutions.Landing.API/appsettings.json" "Landing API"

# Verificar Extra Hours API
check_jwt_config "apps/extra-hours/backend/src/JEGASolutions.ExtraHours.API/appsettings.json" "Extra Hours API"

# Verificar Report Builder API
check_jwt_config "apps/report-builder/backend/src/JEGASolutions.ReportBuilder.API/appsettings.json" "Report Builder API"

# Verificar Tenant Dashboard API (no existe backend)
echo "📁 Verificando: Tenant Dashboard API"
echo "   ⚠️  No tiene backend - solo frontend"
echo "---"

echo ""
echo "✅ Validación completada"
