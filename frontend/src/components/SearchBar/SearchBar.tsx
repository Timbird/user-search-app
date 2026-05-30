import React, { useRef } from 'react';
import { AutocompleteDropdown } from '../AutocompleteDropdown/AutocompleteDropdown';
import './SearchBar.scss';

interface Props {
  query: string;
  suggestions: string[];
  onChange: (q: string) => void;
  onSearch: (q: string) => void;
  onSelect: (name: string) => void;
}

export function SearchBar({ query, suggestions, onChange, onSearch, onSelect }: Props) {
  const inputRef = useRef<HTMLInputElement>(null);

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') onSearch(query);
  };

  return (
    <div className="search-bar">
      <div className="search-bar__input-wrap">
        <input
          ref={inputRef}
          className="search-bar__input"
          type="text"
          placeholder="Search for a user..."
          value={query}
          onChange={e => onChange(e.target.value)}
          onKeyDown={handleKeyDown}
          autoComplete="off"
        />
        {suggestions.length > 0 && (
          <AutocompleteDropdown
            items={suggestions}
            onSelect={onSelect}
          />
        )}
      </div>
      <button className="search-bar__btn" onClick={() => onSearch(query)}>
        Go!
      </button>
    </div>
  );
}
