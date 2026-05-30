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
    <div className={`app${hasSearched ? ' app--searched' : ''}`}>
      <div className="app__search-section">
        <div className="app__container">
          <SearchBar
            query={query}
            suggestions={suggestions}
            onChange={setQuery}
            onSearch={runSearch}
            onSelect={selectSuggestion}
          />
        </div>
      </div>

      <main className="app__main">
        <div className="app__container">
          <UserCardList users={results} hasSearched={hasSearched} />
        </div>
      </main>

      <div className="app__bottom">
        <div className="app__container">
          {showForm && (
            <NewUserForm
              onSuccess={handleUserCreated}
              onClose={() => setShowForm(false)}
            />
          )}
          <button
            className="app__new-user-btn"
            onClick={() => setShowForm(f => !f)}
          >
            {showForm ? 'Cancel' : 'New User +'}
          </button>
        </div>
      </div>

      {toast && <Toast message={toast} onDismiss={() => setToast(null)} />}
    </div>
  );
}
