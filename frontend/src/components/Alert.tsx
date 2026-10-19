import React from 'react';
import { AlertCircle, CheckCircle2, X } from 'lucide-react';

interface AlertProps {
  type: 'success' | 'error' | 'info';
  message: string;
  onClose: () => void;
}

export const Alert: React.FC<AlertProps> = ({ type, message, onClose }) => {
  const isSuccess = type === 'success';
  const isError = type === 'error';

  return (
    <div
      style={{
        position: 'fixed',
        bottom: '24px',
        right: '24px',
        maxWidth: '420px',
        display: 'flex',
        alignItems: 'center',
        gap: '12px',
        padding: '14px 18px',
        borderRadius: '10px',
        backgroundColor: isSuccess ? '#064e3b' : isError ? '#7f1d1d' : '#1e293b',
        border: `1px solid ${isSuccess ? '#059669' : isError ? '#dc2626' : '#334155'}`,
        color: '#f8fafc',
        boxShadow: '0 8px 24px rgba(0, 0, 0, 0.4)',
        zIndex: 9999,
        animation: 'fadeIn 0.25s ease-out',
      }}
    >
      {isSuccess ? (
        <CheckCircle2 size={22} color="#34d399" />
      ) : (
        <AlertCircle size={22} color="#f87171" />
      )}
      <div style={{ flex: 1, fontSize: '0.9rem', wordBreak: 'break-word' }}>
        {message}
      </div>
      <button
        onClick={onClose}
        style={{
          background: 'transparent',
          color: '#94a3b8',
          padding: '4px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <X size={18} />
      </button>
    </div>
  );
};
