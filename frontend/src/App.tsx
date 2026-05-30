import { useState } from 'react';
import { SearchBar } from './components/SearchBar/SearchBar';
import { UserCardList } from './components/UserCardList/UserCardList';
import { NewUserForm } from './components/NewUserForm/NewUserForm';
import { Toast } from './components/Toast/Toast';
import { useSearch } from './hooks/useSearch';
import type { User } from './types/user';
import './App.scss';

export default function App() {
  const { query, setQuery, suggestions, results, hasSearched, runSearch, selectSuggestion } =
    useSearch();
  const [showForm, setShowForm] = useState(false);
  const [toast, setToast] = useState<string | null>(null);

  const handleUserCreated = (_user: User) => {
    setToast('New user added!');
    setShowForm(false);
  };

  return (
    <div className="app">
      <header className="app__header">
        <div className="app__container">
          <div className="app__top-bar">
            <SearchBar
              query={query}
              suggestions={suggestions}
              onChange={setQuery}
              onSearch={runSearch}
              onSelect={selectSuggestion}
            />
            <button
              className="app__new-user-btn"
              onClick={() => setShowForm(f => !f)}
            >
              {showForm ? 'Cancel' : 'New User +'}
            </button>
          </div>

          {showForm && (
            <NewUserForm
              onSuccess={handleUserCreated}
              onClose={() => setShowForm(false)}
            />
          )}
        </div>
      </header>

      <main className="app__main">
        <div className="app__container">
          <UserCardList users={results} hasSearched={hasSearched} />
        </div>
      </main>

      {toast && <Toast message={toast} onDismiss={() => setToast(null)} />}
    </div>
  );
}
