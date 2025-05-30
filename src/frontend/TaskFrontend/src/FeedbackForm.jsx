import React, { useState } from 'react';
import axios from 'axios';
import './FeedbackForm.css';

const FeedbackForm = () => {
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        message: ''
    });

    const [success, setSuccess] = useState('');
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const [generalError, setGeneralError] = useState('');

    const validateForm = () => {
        const newErrors = {};
        if (!formData.name.trim()) {
            newErrors.name = "İsim alanı boş bırakılamaz.";
        } else if (formData.name.trim().length < 2) {
            newErrors.name = "İsim alanı 2 karakterden fazla olmalıdır.";
        } else if (formData.name.trim().length > 20) {
            newErrors.name = "İsim alanı 20 karakterden az olmalıdır.";
        }

        if (!formData.email.trim()) {
            newErrors.email = "Email alanı boş bırakılamaz.";
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = "Lütfen geçerli bir e-posta giriniz.";
        }

        if (!formData.message.trim()) {
            newErrors.message = "Mesaj alanı boş bırakılamaz.";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleChange = e => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        setErrors({ ...errors, [e.target.name]: '' });
        setSuccess('');
        setGeneralError('');
    };

    const handleSubmit = async e => {
        e.preventDefault();

        setGeneralError('');
        if (!validateForm()) return;

        setLoading(true);

        try {
            await axios.post('https://localhost:7077/api/Feedback', formData);
            setSuccess('Geri bildiriminiz alındı.');
            setErrors({});
            setFormData({ name: '', email: '', message: '' });

            setTimeout(() => {
                setSuccess('');
            }, 4000);
        } catch (error) {
            console.error("Gönderim hatası:", error);
            if (error.response?.data?.errors) {
                const apiErrors = {};
                const errorsData = error.response.data.errors;
                for (const key in errorsData) {
                    apiErrors[key.toLowerCase()] = errorsData[key][0];
                }
                setErrors(apiErrors);
            }
            else {
                setGeneralError({ general: "Bir hata oluştu, lütfen tekrar deneyin." });
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="form-container">
            <h2>Geri Bildirim Formu</h2>
            <form onSubmit={handleSubmit} noValidate>
                <input
                    type="text"
                    name="name"
                    placeholder="Adınız"
                    value={formData.name}
                    onChange={handleChange}
                    aria-invalid={errors.name ? "true" : "false"}
                />
                <div className="error-message">{errors.name}</div>

                <input
                    type="email"
                    name="email"
                    placeholder="E-posta"
                    value={formData.email}
                    onChange={handleChange}
                    aria-invalid={errors.email ? "true" : "false"}
                />
                <div className="error-message">{errors.email}</div>

                <textarea
                    name="message"
                    placeholder="Mesajınız"
                    value={formData.message}
                    onChange={handleChange}
                    aria-invalid={errors.message ? "true" : "false"}
                />
                <div className="error-message">{errors.message}</div>

                <button type="submit" disabled={loading}>
                    {loading ? <span className="spinner" aria-label="Yükleniyor"></span> : 'Gönder'}
                </button>

                {success && <div className="success-toast">{success}</div>}
                {generalError && <div className="error-message" role="alert">{generalError}</div>}
            </form>
        </div>
    );
};

export default FeedbackForm;
